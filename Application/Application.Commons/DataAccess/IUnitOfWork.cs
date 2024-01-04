using Application.Commons.DataAccess.ReadOnly;

namespace Application.Commons.DataAccess;

public interface IUnitOfWork : IDisposable
{
    
    public ICategoryReadOnlyRepository CategoryReadOnlyRepository { get; set; }
    public ICategoryRepository CategoryRepository { get; set; }
    bool HasActiveTransaction { get; }

    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    Task SaveChangesAsync();
}