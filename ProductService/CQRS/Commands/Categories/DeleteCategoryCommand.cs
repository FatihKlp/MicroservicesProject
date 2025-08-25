using MediatR;

namespace ProductService.CQRS.Commands.Categories
{
    public record DeleteCategoryCommand(int Id) : IRequest<bool>;
}
