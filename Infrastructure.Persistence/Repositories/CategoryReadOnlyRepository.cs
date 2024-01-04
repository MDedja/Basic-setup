using Application.Commons.DataAccess.ReadOnly;
using AutoMapper;
using Entities.Category.ReadModels;
using Infrastructure.Persistence.Records.CategoryRecord;
using Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CategoryReadOnlyRepository : ReadOnlyRepository<CategoryReadModel, CategoryRecord>, ICategoryReadOnlyRepository
    {
        public CategoryReadOnlyRepository(DatabaseContext context, IMapper mapper) : base(context, mapper)
        {
        }

        protected override IQueryable<CategoryRecord> BaseQuery => _context.Categories.AsNoTracking();
        protected override IQueryable<CategoryRecord> BaseBrowseQuery => BaseQuery;
    }
}