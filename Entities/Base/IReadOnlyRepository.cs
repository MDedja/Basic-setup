using System.Linq.Expressions;

namespace Entities.Base
{
    public interface IReadOnlyRepository<TReadModel> where TReadModel : IReadModel
    {
        Task<TReadModel> GetByIdAsync(Guid id);

        bool Exists(Guid id);
        bool Any(Expression<Func<TReadModel, bool>> filter = null);

        Task<List<TReadModel>> GetAllAsync(bool readOnly = false);

        Task<TReadModel> GetSingleAsync(Expression<Func<TReadModel, bool>> filter, bool readOnly = false);

        Task<List<TReadModel>> GetFilteredAsync(Expression<Func<TReadModel, bool>> filter, bool readOnly = false);

        // Task<PaginatedListReadModel<TReadModel>> BrowsePaginatedReadModel(PageQuery query);
        Task<bool> ExistFilteredAsync(Expression<Func<TReadModel, bool>> filter);

    }
}
