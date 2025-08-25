using MediatR;
using Shared.DTOs;

namespace ProductService.CQRS.Queries.Products
{
    public class GetProductsQuery : IRequest<IEnumerable<ProductDto>> { }
}
