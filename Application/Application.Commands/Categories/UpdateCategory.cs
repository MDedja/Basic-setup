using Application.Commons.DataAccess;
using AutoMapper;
using Entities.Category;
using MediatR;

namespace Application.Commands.Categories
{
    public static class UpdateCategory
    {
        #region Command

        public record Command3(Guid Id, string Code, string Name) : IRequest<Response3>;

        #endregion

        #region Handler

        public class Handler : IRequestHandler<Command3, Response3>
        {
            private readonly IUnitOfWork _unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<Response3> Handle(Command3 request, CancellationToken cancellationToken)
            {
                var region = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
                UpdateCategoryObject(region, request);

                await _unitOfWork.CategoryRepository.UpdateAsync(region);

                await _unitOfWork.SaveChangesAsync();

                return new Response3();
            }

            private static void UpdateCategoryObject(Category category, Command3 request)
            {
                category.Code = request.Code;
                category.Name = request.Name;
            }
        }

        #endregion

        #region Response

        public record Response3();

        #endregion
    }
}