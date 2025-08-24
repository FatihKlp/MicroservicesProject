using Shared.DTOs;

namespace ProductService.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(ProductCreateDto productDto);
        Task<bool> UpdateAsync(int id, ProductCreateDto productDto);
        Task<bool> DeleteAsync(int id);
    }
}
