using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Product name is required")]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public int? CategoryId { get; set; }
    }
}
