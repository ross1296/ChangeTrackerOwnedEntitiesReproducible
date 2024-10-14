using ChangeTrackerOwnedEntitiesReproducible.Domain;
using Microsoft.EntityFrameworkCore;

namespace ChangeTrackerOwnedEntitiesReproducible.Infrastructure.Context
{
    public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : DbContext(options)
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UnitOfWork.AuditLog> AuditLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.HasDefaultSchema(Schemas.Default);
        }
    }
}
