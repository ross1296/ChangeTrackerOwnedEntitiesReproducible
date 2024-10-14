using System.Reflection;
using System.Text.Json;
using ChangeTrackerOwnedEntitiesReproducible.Domain;
using ChangeTrackerOwnedEntitiesReproducible.Domain.ValueObject;
using ChangeTrackerOwnedEntitiesReproducible.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace ChangeTrackerOwnedEntitiesReproducible.Infrastructure
{
    public class UnitOfWork() : IUnitOfWork
    {
        public List<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext) : this()
        {
            _dbContext = dbContext;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                List<AuditEntry> auditEntries = OnBeforeSaveChanges();
                int result = await _dbContext.SaveChangesAsync(cancellationToken);
                OnAfterSaveChanges(auditEntries);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving changes.", ex);
            }
        }

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            _dbContext.ChangeTracker.DetectChanges();
            List<AuditEntry> auditEntries = new List<AuditEntry>();

            foreach (EntityEntry entry in _dbContext.ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                {
                    continue;
                }
                
                AuditEntry auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Entity.GetType().Name,
                    UserId = Guid.NewGuid().ToString() // Replace with proper implementation
                };
                auditEntries.Add(auditEntry);

                foreach (PropertyEntry property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = GetValueFromValueObject(property.CurrentValue);
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = GetValueFromValueObject(property.CurrentValue);
                            break;

                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = GetValueFromValueObject(property.CurrentValue);
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.OldValues[propertyName] = GetValueFromValueObject(property.OriginalValue);
                                auditEntry.NewValues[propertyName] = GetValueFromValueObject(property.CurrentValue);
                            }

                            break;
                    }
                }
                
                // Handle owned entities
                foreach (NavigationEntry navigationEntry in entry.Navigations.Where(n => n.Metadata.TargetEntityType.IsOwned() && (n.CurrentValue == null || n.CurrentValue is ValueObject)))
                {
                    if (navigationEntry is CollectionEntry collectionEntry)
                    {
                        // Handle collection of owned entities
                        foreach (object? ownedEntry in collectionEntry.CurrentValue)
                        {
                            HandleOwnedEntity(ownedEntry, entry, auditEntry);
                        }
                    }
                    else
                    {
                        // Handle single owned entity
                        object? ownedEntry = navigationEntry.CurrentValue;
                        if (ownedEntry != null)
                        {
                            HandleOwnedEntity(ownedEntry, entry, auditEntry);
                        }
                    }
                }
            }

            foreach (AuditEntry auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                try
                {
                    AuditLogs.Add(auditEntry.ToAudit());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        }
        
        private void HandleOwnedEntity(object ownedEntry, EntityEntry parentEntry, AuditEntry parentAuditEntry)
        {
            PropertyInfo[] properties = ownedEntry.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                string propertyName = $"{parentEntry.Metadata.GetTableName()}.{property.Name}";
                if (parentEntry.State == EntityState.Added)
                {
                    parentAuditEntry.NewValues[propertyName] = property.GetValue(ownedEntry);
                }
                else if (parentEntry.State == EntityState.Deleted)
                {
                    parentAuditEntry.OldValues[propertyName] = property.GetValue(ownedEntry);
                }
                else if (parentEntry.State == EntityState.Modified)
                {
                    object? originalValue = property.GetValue(ownedEntry);
                    object? currentValue = property.GetValue(ownedEntry);

                    if (!Equals(originalValue, currentValue))
                    {
                        parentAuditEntry.OldValues[propertyName] = originalValue;
                        parentAuditEntry.NewValues[propertyName] = currentValue;
                    }
                }
            }
        }

        private async void OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
            {
                return;
            }

            foreach (AuditEntry auditEntry in auditEntries)
            {
                foreach (PropertyEntry prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                AuditLogs.Add(auditEntry.ToAudit());
            }

            await SaveChangesAsync();
        }
        
        private object GetValueFromValueObject(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            PropertyInfo valueProperty = obj.GetType().GetProperty("Value");
            if (valueProperty != null)
            {
                return valueProperty.GetValue(obj);
            }
            
            return obj;
        }

        public class AuditLog
        {
            public int Id { get; set; }
            public string UserId { get; set; }
            public string Type { get; set; }
            public string TableName { get; set; }
            public DateTime DateTime { get; set; }
            public string OldValues { get; set; }
            public string NewValues { get; set; }
            public string AffectedColumns { get; set; }
            public string PrimaryKey { get; set; }
        }

        public class AuditEntry : Entity<AuditEntry>
        {
            public AuditEntry(EntityEntry entry)
            {
                Entry = entry;
                TemporaryProperties = new List<PropertyEntry>();
            }

            public AuditEntry()
            {
                
            }

            public EntityEntry Entry { get; }
            public string UserId { get; set; }
            public string TableName { get; set; }
            public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
            public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
            public Dictionary<string, object>? NewValues { get; } = new Dictionary<string, object>();
            public List<PropertyEntry> TemporaryProperties { get; }
            public bool HasTemporaryProperties => TemporaryProperties.Any();

            public AuditLog ToAudit()
            {
                AuditLog audit = new AuditLog
                {
                    UserId = UserId,
                    Type = Entry.State.ToString(),
                    TableName = TableName,
                    DateTime = DateTime.UtcNow,
                    PrimaryKey = JsonSerializer.Serialize(KeyValues),
                    OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues),
                    NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues),
                    AffectedColumns = JsonSerializer.Serialize(OldValues.Keys.Union(NewValues.Keys).Distinct().ToList())
                };
                return audit;
            }
        }
    }
}
