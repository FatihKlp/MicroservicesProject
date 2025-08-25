using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.CQRS.Queries.Categories;
using ProductService.Repositories;
using Shared.DTOs;
using System.Text.Json;

namespace ProductService.CQRS.Handlers.Categories
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, List<CategoryWithProductsDto>>
    {
        private readonly ICategoryRepository _repository;
        private readonly IDistributedCache _cache;

        private const string CacheKey = "categories_all";

        public GetCategoriesHandler(ICategoryRepository repository, IDistributedCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<List<CategoryWithProductsDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            List<CategoryWithProductsDto>? cachedResult = null;
            try
            {
                var cached = await _cache.GetStringAsync(CacheKey, cancellationToken);
                if (!string.IsNullOrEmpty(cached))
                {
                    cachedResult = JsonSerializer.Deserialize<List<CategoryWithProductsDto>>(cached);
                }
            }
            catch
            {
                // Redis yoksa/yetkisizse cache'i atla
            }

            if (cachedResult != null)
            {
                return cachedResult;
            }

            var categories = await _repository.GetAllAsync();
            var result = categories.Select(c => new CategoryWithProductsDto
            {
                Id = c.Id,
                Name = c.Name,
                Products = c.Products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                }).ToList()
            }).ToList();

            try
            {
                await _cache.SetStringAsync(CacheKey, JsonSerializer.Serialize(result),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) }, cancellationToken);
            }
            catch
            {
                // Cache yazılamazsa ignore
            }

            return result;
        }
    }
}
