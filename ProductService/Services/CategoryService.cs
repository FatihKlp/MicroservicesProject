using ProductService.Models;
using ProductService.Repositories;
using ProductService.Interfaces;

namespace ProductService.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository repository, ILogger<CategoryService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Category> CreateAsync(string name)
        {
            var category = new Category { Name = name };
            await _repository.AddAsync(category);
            _logger.LogInformation("Category created: {Name}", name);
            return category;
        }

        public async Task<bool> UpdateAsync(int id, string name)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return false;

            category.Name = name;
            await _repository.UpdateAsync(category);
            _logger.LogInformation("Category updated: {Id}", id);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id == 1) // Uncategorized silinemez
            {
                _logger.LogWarning("Tried to delete Uncategorized category");
                return false;
            }

            var category = await _repository.GetByIdAsync(id);
            if (category == null) return false;

            await _repository.DeleteAsync(category);
            _logger.LogInformation("Category deleted: {Id}", id);
            return true;
        }
    }
}
