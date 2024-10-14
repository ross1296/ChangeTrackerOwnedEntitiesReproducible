using Microsoft.EntityFrameworkCore.Storage;

namespace ChangeTrackerOwnedEntitiesReproducible.Infrastructure;

public interface IUnitOfWork
{
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}