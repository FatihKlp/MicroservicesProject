using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.CQRS.Commands.Products;
using ProductService.CQRS.Queries.Products;
using Shared.DTOs;
using Shared.Interfaces;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogServiceClient _logService;

        public ProductsController(IMediator mediator, ILogServiceClient logService)
        {
            _mediator = mediator;
            _logService = logService;
        }

        // GET: api/products
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var result = await _mediator.Send(new GetProductsQuery());

            await _logService.SendLogAsync(new LogEntryDto
            {
                Service = "ProductService",
                Level = "Info",
                Message = $"Fetched {result.Count()} products",
                UserId = User?.Identity?.Name
            });

            return Ok(result);
        }

        // GET: api/products/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id));
            if (result == null)
            {
                await _logService.SendLogAsync(new LogEntryDto
                {
                    Service = "ProductService",
                    Level = "Warning",
                    Message = $"Product with id {id} not found",
                    UserId = User?.Identity?.Name
                });
                return NotFound();
            }

            await _logService.SendLogAsync(new LogEntryDto
            {
                Service = "ProductService",
                Level = "Info",
                Message = $"Product fetched: {result.Name}",
                UserId = User?.Identity?.Name
            });

            return Ok(result);
        }

        // POST: api/products
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductCreateDto productDto)
        {
            var result = await _mediator.Send(new CreateProductCommand(productDto));

            await _logService.SendLogAsync(new LogEntryDto
            {
                Service = "ProductService",
                Level = "Info",
                Message = $"Product created: {result.Name}",
                UserId = User?.Identity?.Name
            });

            return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
        }

        // PUT: api/products/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductCreateDto productDto)
        {
            var success = await _mediator.Send(new UpdateProductCommand(id, productDto));
            if (!success)
            {
                await _logService.SendLogAsync(new LogEntryDto
                {
                    Service = "ProductService",
                    Level = "Warning",
                    Message = $"Failed to update product {id}",
                    UserId = User?.Identity?.Name
                });
                return NotFound();
            }

            await _logService.SendLogAsync(new LogEntryDto
            {
                Service = "ProductService",
                Level = "Info",
                Message = $"Product updated: {id}",
                UserId = User?.Identity?.Name
            });

            return NoContent();
        }

        // DELETE: api/products/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var success = await _mediator.Send(new DeleteProductCommand(id));
            if (!success)
            {
                await _logService.SendLogAsync(new LogEntryDto
                {
                    Service = "ProductService",
                    Level = "Warning",
                    Message = $"Failed to delete product {id}",
                    UserId = User?.Identity?.Name
                });
                return NotFound();
            }

            await _logService.SendLogAsync(new LogEntryDto
            {
                Service = "ProductService",
                Level = "Info",
                Message = $"Product deleted: {id}",
                UserId = User?.Identity?.Name
            });

            return NoContent();
        }
    }
}
