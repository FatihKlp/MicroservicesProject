using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.CQRS.Commands.Categories;
using ProductService.Models;
using ProductService.Repositories;
using Shared.DTOs;

namespace ProductService.CQRS.Handlers.Categories
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryRepository _repository;
        private readonly IDistributedCache _cache;

        public CreateCategoryHandler(ICategoryRepository repository, IDistributedCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Category { Name = request.Dto.Name };
            await _repository.AddAsync(category);

            // Cache invalidation
            await _cache.RemoveAsync("categories_all", cancellationToken);

            return new CategoryDto { Id = category.Id, Name = category.Name };
        }
    }
}
