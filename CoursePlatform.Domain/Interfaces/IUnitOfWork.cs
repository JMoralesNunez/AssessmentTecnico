namespace CoursePlatform.Domain.Interfaces;

public interface IDbTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
