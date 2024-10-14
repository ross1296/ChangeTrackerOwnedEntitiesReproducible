using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChangeTrackerOwnedEntitiesReproducible.Infrastructure.Configurations
{
    public class AuditEntryConfiguration : IEntityTypeConfiguration<UnitOfWork.AuditEntry>
    {
        public void Configure(EntityTypeBuilder<UnitOfWork.AuditEntry> builder)
        {
            builder.HasKey(ae => ae.Id);

            builder.Property(ae => ae.UserId).IsRequired();
            builder.Property(ae => ae.TableName).IsRequired();

            builder.Property(ae => ae.KeyValues)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null));

            builder.Property(ae => ae.OldValues)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions)null) : null,
                    v => v != null ? JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null) : null)
                .IsRequired(false);

            builder.Property(ae => ae.NewValues)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions)null) : null,
                    v => v != null ? JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null) : null)
                .IsRequired(false);

            builder.Ignore(ae => ae.Entry);
            builder.Ignore(ae => ae.TemporaryProperties);
            builder.Ignore(ae => ae.HasTemporaryProperties);
        }
    }
}