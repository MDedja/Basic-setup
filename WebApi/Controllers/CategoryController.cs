using Application.Commands.Categories;
using Application.Queries.Category;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator) =>
        _mediator = mediator;
    
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<GetCategory.Response>> GetCategoryAsync(Guid id) =>
        await _mediator.Send(new GetCategory.Query(id));
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<CreateCategory.Response2>> CreateCategoryAsync(CreateCategory.Command2 request) =>
        await _mediator.Send(request);
    
    [AllowAnonymous]
    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateCategory.Response3>> UpdateCategoryAsync(Guid id, UpdateCategory.Command3 request)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }
    
        return await _mediator.Send(request);
    }
    
    [AllowAnonymous]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DeleteCategory.Response1>> DeleteCategoryAsync(Guid id) =>
        await _mediator.Send(new DeleteCategory.Command1(id));
}