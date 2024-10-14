using ChangeTrackerOwnedEntitiesReproducible.Domain;
using ChangeTrackerOwnedEntitiesReproducible.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChangeTrackerOwnedEntitiesReproducible.Infrastructure.Configurations
{
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(x => x.Title).IsRequired(false);
            
            builder.Property(x => x.Forename)
                .HasConversion(x => x!.Value, v => Name.Create(v))
                .IsRequired();
            
            builder.Property(x => x.Surname)
                .HasConversion(x => x!.Value, v => Name.Create(v))
                .IsRequired();
            
            builder.Property(x => x.Email)
                .HasConversion(x => x!.Value, v => Email.Create(v))
                .IsRequired(false);
            
            builder.Property(x => x.MobilePhoneNumber)
                .HasConversion(x => x!.Value, v => PhoneNumber.Create(v))
                .IsRequired(false);
        }
    }
}