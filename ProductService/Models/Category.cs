namespace ProductService.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation property (one-to-many relationship)
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
