using Application.Commons.DataAccess;
using AutoMapper;
using Entities.Category.ReadModels;
using MediatR;

namespace Application.Queries.Category;

public static class GetCategory
{
    public record Query(Guid Id) : IRequest<Response>;

    public class Handler : IRequestHandler<Query, Response>
    {
        private readonly IMapper _mapper;
        private IUnitOfWork _unitOfWork;

        public Handler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            if (!_unitOfWork.CategoryReadOnlyRepository.Exists(request.Id))
            {
                throw new Exception("Ne postoji kategorija sa datim id-jem");
            }
            var category = await _unitOfWork.CategoryReadOnlyRepository.GetByIdAsync(request.Id);

            return _mapper.Map<Response>(category);
        }
    }

    public record Response(Guid Id, string Code, string Name);

    #region Mappings

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CategoryReadModel, Response>();
        }
    }

    #endregion
}