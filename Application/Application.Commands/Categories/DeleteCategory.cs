using Application.Commons.DataAccess;
using MediatR;

namespace Application.Commands.Categories;

public class DeleteCategory
{
    #region Command

    public record Command1(Guid Id) : IRequest<Response1>;

    #endregion

    #region Handler

    public class Handler : IRequestHandler<Command1, Response1>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response1> Handle(Command1 request, CancellationToken cancellationToken)
        {
            var region = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);

            await _unitOfWork.CategoryRepository.RemoveAsync(region);

            await _unitOfWork.SaveChangesAsync();

            return new Response1();
        }
    }

    #endregion

    #region Response

    public record Response1();

    #endregion
}