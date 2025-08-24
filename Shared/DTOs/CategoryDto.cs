using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [MinLength(2, ErrorMessage = "Category name must be at least 2 characters")]
        public string Name { get; set; } = string.Empty;
    }

    public class CategoryWithProductsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ProductDto> Products { get; set; } = new();
    }

}
