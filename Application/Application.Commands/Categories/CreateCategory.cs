using Application.Commons.DataAccess;
using AutoMapper;
using Entities.Category;
using MediatR;

namespace Application.Commands.Categories
{
    public static class CreateCategory
    {
        #region Command

        public record Command2(string Code, string Name) : IRequest<Response2>;

        #endregion

        #region Handler

        public class Handler : IRequestHandler<Command2, Response2>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public Handler(
                IUnitOfWork unitOfWork,
                IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<Response2> Handle(Command2 request, CancellationToken cancellationToken)
            {

                var category = new Category(Guid.NewGuid(), request.Code, request.Name);

                await _unitOfWork.CategoryRepository.AddAsync(category);

                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<Response2>(category);
            }
        }

        #endregion

        #region Response

        public record Response2(
            Guid Id,
            string Code,
            string Name);

        #endregion

        #region Mappings

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Category, Response2>();
            }
        }

        #endregion
    }
}
