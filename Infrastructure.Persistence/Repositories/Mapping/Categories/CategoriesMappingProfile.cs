using AutoMapper;
using Entities.Category;
using Entities.Category.ReadModels;
using Infrastructure.Persistence.Records.CategoryRecord;

namespace Infrastructure.Persistence.Repositories.Mapping.Categories
{
    public class CategoriesMappingProfile : Profile
    {
        public CategoriesMappingProfile()
        {
            CreateMap<Category, CategoryRecord>()
                .ReverseMap();
            CreateMap<CategoryReadModel, CategoryRecord>()
                .ReverseMap();
        }
    }
}