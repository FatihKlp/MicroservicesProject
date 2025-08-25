using MediatR;
using Shared.DTOs;

namespace ProductService.CQRS.Commands.Categories
{
    public record UpdateCategoryCommand(int Id, CategoryCreateDto Dto) : IRequest<bool>;
}