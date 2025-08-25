using MediatR;
using Shared.DTOs;

namespace ProductService.CQRS.Queries.Categories
{
    public class GetCategoriesQuery : IRequest<List<CategoryWithProductsDto>> { }
}
