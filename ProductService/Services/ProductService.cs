using Shared.DTOs;
using ProductService.Models;
using ProductService.Repositories;
using ProductService.Interfaces;

namespace ProductService.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository repository, ILogger<ProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            try
            {
                var products = await _repository.GetAllAsync();
                return products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve all products");
                throw;
            }
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            try
            {
                var p = await _repository.GetByIdAsync(id);
                if (p == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", id);
                    return null;
                }

                return new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve product with ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<ProductDto> CreateAsync(ProductCreateDto productDto)
        {
            try
            {
                var product = new Product
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    CategoryId = productDto.CategoryId ?? 1 // null ise Uncategorized
                };

                await _repository.AddAsync(product);
                _logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);

                return new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create product: {ProductName}", productDto.Name);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, ProductCreateDto productDto)
        {
            try
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Cannot update - product with ID {ProductId} not found", id);
                    return false;
                }

                existing.Name = productDto.Name;
                existing.Price = productDto.Price;
                existing.CategoryId = productDto.CategoryId ?? existing.CategoryId;

                await _repository.UpdateAsync(existing);
                _logger.LogInformation("Product updated successfully: {ProductId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update product with ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Cannot delete - product with ID {ProductId} not found", id);
                    return false;
                }

                await _repository.DeleteAsync(existing);
                _logger.LogInformation("Product deleted successfully: {ProductId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product with ID: {ProductId}", id);
                throw;
            }
        }
    }
}