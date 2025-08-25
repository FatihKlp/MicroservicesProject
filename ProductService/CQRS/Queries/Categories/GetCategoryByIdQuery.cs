using MediatR;
using Shared.DTOs;

namespace ProductService.CQRS.Queries.Categories
{
    public record GetCategoryByIdQuery(int Id) : IRequest<CategoryWithProductsDto?>;
}
