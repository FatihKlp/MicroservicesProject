using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.CQRS.Queries.Products;
using ProductService.Data;
using Shared.DTOs;
using System.Text.Json;
using ProductService.Cache;

namespace ProductService.CQRS.Handlers.Products
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public GetProductsHandler(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            // Redis cache'i geçici olarak devre dışı bırakıyoruz
            // var cacheKey = CacheKeys.ProductsAll;
            // var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
            // if (!string.IsNullOrEmpty(cached))
            //     return JsonSerializer.Deserialize<List<ProductDto>>(cached)!;

            var products = await _context.Products
                .AsNoTracking()
                .Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price })
                .ToListAsync(cancellationToken);

            // Cache'i geçici olarak devre dışı bırakıyoruz
            // await _cache.SetStringAsync(
            //     cacheKey,
            //     JsonSerializer.Serialize(products),
            //     new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) },
            //     cancellationToken
            // );

            return products;
        }
    }
}
