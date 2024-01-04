using Application.Commons.DataAccess;
using AutoMapper;
using Entities.Category;
using Infrastructure.Persistence.Records.CategoryRecord;
using Infrastructure.Persistence.Repositories.Base;

namespace Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : Repository<Category, CategoryRecord>, ICategoryRepository
    {
        public CategoryRepository(DatabaseContext context, IMapper mapper) : base(context, mapper)
        {
        }
        protected override IQueryable<CategoryRecord> BaseQuery => _context.Categories;
    }
}