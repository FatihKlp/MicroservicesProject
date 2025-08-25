using MediatR;
using Shared.DTOs;

namespace ProductService.CQRS.Commands.Categories
{
    public record CreateCategoryCommand(CategoryCreateDto Dto) : IRequest<CategoryDto>;
}
