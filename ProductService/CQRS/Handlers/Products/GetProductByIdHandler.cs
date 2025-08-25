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
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public GetProductByIdHandler(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.ProductById(request.Id);
            var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cached))
                return JsonSerializer.Deserialize<ProductDto>(cached);

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product == null) return null;

            var dto = new ProductDto { Id = product.Id, Name = product.Name, Price = product.Price };

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(dto),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) },
                cancellationToken
            );

            return dto;
        }
    }
}
