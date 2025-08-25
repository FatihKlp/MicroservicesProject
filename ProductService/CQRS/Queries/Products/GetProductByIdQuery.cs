using MediatR;
using Shared.DTOs;

namespace ProductService.CQRS.Queries.Products
{
    public class GetProductByIdQuery : IRequest<ProductDto?>
    {
        public int Id { get; set; }

        public GetProductByIdQuery(int id)
        {
            Id = id;
        }
    }
}
