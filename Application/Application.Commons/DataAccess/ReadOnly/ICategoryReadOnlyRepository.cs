using Entities.Base;
using Entities.Category.ReadModels;

namespace Application.Commons.DataAccess.ReadOnly
{
    public interface ICategoryReadOnlyRepository : IReadOnlyRepository<CategoryReadModel>
    {
    }
}