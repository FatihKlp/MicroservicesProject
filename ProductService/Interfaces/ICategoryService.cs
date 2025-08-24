using ProductService.Models;

namespace ProductService.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> CreateAsync(string name);
        Task<bool> UpdateAsync(int id, string name);
        Task<bool> DeleteAsync(int id);
    }
}
