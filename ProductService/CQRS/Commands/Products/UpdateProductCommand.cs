using MediatR;
using Shared.DTOs;

namespace ProductService.CQRS.Commands.Products
{
    public class UpdateProductCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public ProductCreateDto Product { get; set; }

        public UpdateProductCommand(int id, ProductCreateDto product)
        {
            Id = id;
            Product = product;
        }
    }
}
