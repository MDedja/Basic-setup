using Entities.Base;
using Entities.Category;
using Entities.Category.ReadModels;
using Infrastructure.Persistence.Records.Base;

namespace Infrastructure.Persistence.Records.CategoryRecord
{
    public class CategoryRecord : IRecord, IMapFrom<Category>, IMapFromReadModel<CategoryReadModel>
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}