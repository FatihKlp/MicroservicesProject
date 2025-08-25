using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.CQRS.Commands.Products;
using ProductService.Data;
using System.Text.Json;
using ProductService.Cache;

namespace ProductService.CQRS.Handlers.Products
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public UpdateProductHandler(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (entity == null) return false;

            entity.Name = request.Product.Name;
            entity.Price = request.Product.Price;
            entity.CategoryId = request.Product.CategoryId ?? entity.CategoryId;

            await _context.SaveChangesAsync(cancellationToken);

            // Invalidate
            await _cache.RemoveAsync(CacheKeys.ProductsAll, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.ProductById(request.Id), cancellationToken);

            return true;
        }
    }
}
