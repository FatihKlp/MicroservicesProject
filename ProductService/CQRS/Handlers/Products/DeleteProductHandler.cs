using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.CQRS.Commands.Products;
using ProductService.Data;
using ProductService.Cache;

namespace ProductService.CQRS.Handlers.Products
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public DeleteProductHandler(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (entity == null) return false;

            _context.Products.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            // Invalidate
            await _cache.RemoveAsync(CacheKeys.ProductsAll, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.ProductById(request.Id), cancellationToken);

            return true;
        }
    }
}
