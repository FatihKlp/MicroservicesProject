using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.CQRS.Commands.Products;
using ProductService.Data;
using ProductService.Models;
using Shared.DTOs;
using System.Text.Json;
using ProductService.Cache;

namespace ProductService.CQRS.Handlers.Products
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public CreateProductHandler(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var entity = new Product
            {
                Name = request.Product.Name,
                Price = request.Product.Price,
                CategoryId = request.Product.CategoryId ?? 1 // varsa varsayılan
            };

            _context.Products.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var dto = new ProductDto { Id = entity.Id, Name = entity.Name, Price = entity.Price };

            // 1) Liste cache'ini sil
            await _cache.RemoveAsync(CacheKeys.ProductsAll, cancellationToken);

            // 2) Opsiyonel: tekil ürünü cache’e koy (isteğe bağlı)
            await _cache.SetStringAsync(
                CacheKeys.ProductById(entity.Id),
                JsonSerializer.Serialize(dto),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) },
                cancellationToken
            );

            return dto;
        }
    }
}
