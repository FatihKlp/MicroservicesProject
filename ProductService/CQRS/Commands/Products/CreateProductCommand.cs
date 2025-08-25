using MediatR;
using Shared.DTOs;

namespace ProductService.CQRS.Commands.Products
{
    public class CreateProductCommand : IRequest<ProductDto>
    {
        public ProductCreateDto Product { get; set; }

        public CreateProductCommand(ProductCreateDto product)
        {
            Product = product;
        }
    }
}
