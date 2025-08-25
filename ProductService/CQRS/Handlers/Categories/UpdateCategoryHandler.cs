using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.CQRS.Commands.Categories;
using ProductService.Repositories;

namespace ProductService.CQRS.Handlers.Categories
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, bool>
    {
        private readonly ICategoryRepository _repository;
        private readonly IDistributedCache _cache;

        public UpdateCategoryHandler(ICategoryRepository repository, IDistributedCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _repository.GetByIdAsync(request.Id);
            if (category == null) return false;

            category.Name = request.Dto.Name;
            await _repository.UpdateAsync(category);

            // Cache invalidation
            await _cache.RemoveAsync("categories_all", cancellationToken);
            await _cache.RemoveAsync($"category_{request.Id}", cancellationToken);

            return true;
        }
    }
}
