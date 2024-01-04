using System.Data;
using Application.Commons.DataAccess;
using Application.Commons.DataAccess.ReadOnly;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext _dbContext;
    private IDbContextTransaction _currentTransaction;

    public ICategoryReadOnlyRepository CategoryReadOnlyRepository { get; set; }
    public ICategoryRepository CategoryRepository { get; set; }

    public UnitOfWork(DatabaseContext dbContext,
        ICategoryReadOnlyRepository categoryReadOnlyRepository,
        ICategoryRepository categoryRepository)

    {
        _dbContext = dbContext;
        CategoryReadOnlyRepository = categoryReadOnlyRepository;
        CategoryRepository = categoryRepository;
    }


    public void Dispose()
    {
        _dbContext.Dispose();
    }

    /// <summary>
    ///     Saves all changes to tracked entities.
    ///     If an explicit transaction has not yet been started, the
    ///     save operation itself is executed in a new transaction.
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }


    public bool HasActiveTransaction
        => _currentTransaction != null;


    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null) throw new InvalidOperationException("A transaction is already in progress.");

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _dbContext.SaveChangesAsync();

            _currentTransaction?.Commit();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            await _currentTransaction?.RollbackAsync();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }
}