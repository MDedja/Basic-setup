using System.Linq.Expressions;
using AutoMapper;
using Entities.Base;
using Infrastructure.Persistence.Records.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Base;

// TODO: present to team lead generic soft delete method and endpoint with record type
public abstract class Repository<TEntity, TRecord> : IRepository<TEntity> where TEntity : class, IEntity
    where TRecord : class, IRecord, IMapFrom<TEntity>
{
    protected DatabaseContext _context;
    protected DbSet<TRecord> _set;
    protected readonly IMapper _mapper;

    /// <summary>
    /// Defines the base query for the given entity used by all operations.
    /// Concrete implementations should apply all necessary includes and pre-filters here.
    /// </summary>
    protected abstract IQueryable<TRecord> BaseQuery { get; }

    public Repository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _set = context.Set<TRecord>();
        _mapper = mapper;
    }

    public virtual Task AddAsync(TEntity entity)
    {
        var record = _mapper.Map<TRecord>(entity);
        _set.Add(record);
        return Task.CompletedTask;
    }

    public virtual async Task AddRangeAsync(List<TEntity> entities)
    {
        if (!entities.Any())
            return;

        var records = _mapper.Map<List<TRecord>>(entities);

        await _context.AddRangeAsync(records);
    }

    protected virtual void UpdateCollections(TRecord dbRecord, TRecord updateRecord)
    {
    }

    public virtual Task UpdateAsync(TEntity entity)
    {
        var record = _mapper.Map<TRecord>(entity);
        var dbRecord = BaseQuery.Single(x => x.Id == entity.Id);
        _context.Entry(dbRecord).CurrentValues.SetValues(record);
        UpdateCollections(dbRecord, record);
        return Task.CompletedTask;
    }

    public virtual Task UpdateAndActivateAsync(TEntity entity, Guid id)
    {
        // var dbRecord = BaseQuery.Single(x => x.Id == id);
        // var record = _mapper.Map<TRecord>(entity);
        // record.Id = id;
        // var isDeletable = record.GetType().GetInterfaces().Contains(typeof(ISoftDeletable));
        //
        // if (isDeletable)
        // {
        //     var isDeletableRecord = (ISoftDeletable)record;
        //     isDeletableRecord.IsDeleted = false;
        //     _context.Entry(dbRecord).CurrentValues.SetValues(isDeletableRecord);
        // }
        // else
        // {
        //     _context.Entry(dbRecord).CurrentValues.SetValues(record);
        // }
        //
        // UpdateCollections(dbRecord, record);
        return Task.CompletedTask;
    }

    public virtual async Task UpdateRangeAsync(List<TEntity> entities)
    {
        // if (!entities.Any())
        //     return;
        //
        // var records = _mapper.Map<List<TRecord>>(entities);
        //
        // await _context.BulkUpdateAsync(records);
    }

    public virtual Task RemoveAsync(TEntity entityToDelete)
    {
        var record = _mapper.Map<TRecord>(entityToDelete);
        _set.Remove(record);
        return Task.CompletedTask;
    }

    public virtual async Task RemoveRangeAsync(List<TEntity> entitiesToDelete)
    {
        // if (!entitiesToDelete.Any())
        //     return;
        //
        // var records = _mapper.Map<List<TRecord>>(entitiesToDelete);
        //
        // await _context.BulkDeleteAsync(records);
    }

    public virtual bool Exists(Guid id)
    {
        return BaseQuery.Any(e => e.Id == id);
    }
    
    public virtual async Task<TEntity> GetByIdAsync(Guid id)
    {
        var record = await BaseQuery.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id);
        return _mapper.Map<TEntity>(record);
    }

    // public virtual RecordState GetStateByParams(ref Guid id, 
    //     bool isNew, 
    //     params KeyValuePair<string, object>[] parameters)
    // {
    //     IQueryable<TRecord> recordQuery = _set;
    //     var properties = typeof(TRecord).GetProperties();
    //
    //     if (!isNew)
    //     {
    //         var entityId = id;
    //         recordQuery = recordQuery.Where(e => e.Id != entityId);
    //     }
    //     
    //     foreach (var parameter in parameters)
    //     {
    //         var property = properties.FirstOrDefault(e => e.Name == parameter.Key);
    //         recordQuery = recordQuery.Where(x =>
    //             EF.Property<object>(x, property.Name).Equals(parameter.Value));
    //     }
    //
    //     var record = recordQuery.FirstOrDefault();
    //     if (record == null)
    //     {
    //         return isNew ? RecordState.NotFound : RecordState.Unique;
    //     }
    //
    //     var isDeletableProperty = properties.FirstOrDefault(e => e.Name == "IsDeleted");
    //     if (IsRecordActive(isDeletableProperty, record)) return RecordState.Active;
    //
    //     id = record.Id;
    //     return RecordState.Inactive;
    // }

    // private static bool IsRecordActive(PropertyInfo isDeletableProperty, TRecord record)
    // {
    //     if (isDeletableProperty == null)
    //     {
    //         return true;
    //     }
    //
    //     var isDeletedObject = isDeletableProperty.GetValue(record);
    //     var isDeleted = isDeletedObject != null && (bool)isDeletedObject;
    //     
    //     return !isDeleted;
    // }

    public virtual async Task<List<TEntity>> GetAllAsync(bool readOnly = false)
    {
        try
        {
            var records = await (readOnly ? BaseQuery.AsNoTracking() : BaseQuery).ToListAsync();

            return _mapper.Map<List<TEntity>>(records);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter, bool readOnly = false)
    {
        var expressionFilter = _mapper.Map<Expression<Func<TRecord, bool>>>(filter);

        var record = await (readOnly
            ? BaseQuery.AsNoTracking().FirstOrDefaultAsync(expressionFilter)
            : BaseQuery.FirstOrDefaultAsync(expressionFilter));

        if (record == null)
            return null;

        return _mapper.Map<TEntity>(record);
    }

    public async Task<List<TEntity>> GetFilteredAsync(Expression<Func<TEntity, bool>> filter, bool readOnly = false)
    {
        var expressionFilter = _mapper.Map<Expression<Func<TRecord, bool>>>(filter);

        var records =
            await (readOnly ? BaseQuery.AsNoTracking().Where(expressionFilter) : BaseQuery.Where(expressionFilter))
                .ToListAsync();

        return _mapper.Map<List<TEntity>>(records);
    }
}