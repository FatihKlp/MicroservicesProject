using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.CQRS.Queries.Categories;
using ProductService.Repositories;
using Shared.DTOs;
using System.Text.Json;

namespace ProductService.CQRS.Handlers.Categories
{
    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryWithProductsDto?>
    {
        private readonly ICategoryRepository _repository;
        private readonly IDistributedCache _cache;

        public GetCategoryByIdHandler(ICategoryRepository repository, IDistributedCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<CategoryWithProductsDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"category_{request.Id}";

            var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cached))
            {
                return JsonSerializer.Deserialize<CategoryWithProductsDto>(cached)!;
            }

            var category = await _repository.GetByIdAsync(request.Id);
            if (category == null) return null;

            var result = new CategoryWithProductsDto
            {
                Id = category.Id,
                Name = category.Name,
                Products = category.Products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                }).ToList()
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) }, cancellationToken);

            return result;
        }
    }
}
