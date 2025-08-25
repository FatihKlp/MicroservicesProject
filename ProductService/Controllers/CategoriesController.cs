using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.CQRS.Commands.Categories;
using ProductService.CQRS.Queries.Categories;
using Shared.DTOs;
using Shared.Interfaces;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogServiceClient _logService;

        public CategoriesController(IMediator mediator, ILogServiceClient logService)
        {
            _mediator = mediator;
            _logService = logService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryWithProductsDto>>> GetCategories()
        {
            var result = await _mediator.Send(new GetCategoriesQuery());

            await _logService.SendLogAsync(new LogEntryDto
            {
                Service = "ProductService",
                Level = "Info",
                Message = $"Fetched {result.Count()} categories",
                UserId = User?.Identity?.Name
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryWithProductsDto>> GetCategoryById(int id)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery(id));
            if (result == null)
            {
                await _logService.SendLogAsync(new LogEntryDto
                {
                    Service = "ProductService",
                    Level = "Warning",
                    Message = $"Category with id {id} not found",
                    UserId = User?.Identity?.Name
                });
                return NotFound();
            }

            await _logService.SendLogAsync(new LogEntryDto
            {
                Service = "ProductService",
                Level = "Info",
                Message = $"Category fetched: {result.Name}",
                UserId = User?.Identity?.Name
            });

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CategoryCreateDto dto)
        {
            var created = await _mediator.Send(new CreateCategoryCommand(dto));

            await _logService.SendLogAsync(new LogEntryDto
            {
                Service = "ProductService",
                Level = "Info",
                Message = $"Category created: {created.Name}",
                UserId = User?.Identity?.Name
            });

            return CreatedAtAction(nameof(GetCategoryById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryCreateDto dto)
        {
            var success = await _mediator.Send(new UpdateCategoryCommand(id, dto));
            if (!success)
            {
                await _logService.SendLogAsync(new LogEntryDto
                {
                    Service = "ProductService",
                    Level = "Warning",
                    Message = $"Failed to update category {id}",
                    UserId = User?.Identity?.Name
                });
                return NotFound();
            }

            await _logService.SendLogAsync(new LogEntryDto
            {
                Service = "ProductService",
                Level = "Info",
                Message = $"Category updated: {id}",
                UserId = User?.Identity?.Name
            });

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var success = await _mediator.Send(new DeleteCategoryCommand(id));
            if (!success)
            {
                await _logService.SendLogAsync(new LogEntryDto
                {
                    Service = "ProductService",
                    Level = "Warning",
                    Message = $"Failed to delete category {id}",
                    UserId = User?.Identity?.Name
                });
                return BadRequest("Cannot delete category.");
            }

            await _logService.SendLogAsync(new LogEntryDto
            {
                Service = "ProductService",
                Level = "Info",
                Message = $"Category deleted: {id}",
                UserId = User?.Identity?.Name
            });

            return NoContent();
        }
    }
}
