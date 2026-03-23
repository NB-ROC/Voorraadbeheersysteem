namespace Backend.Models
{
    public class Product
    {
        public int Id { get; set; } 

        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public ProductStatus Status { get; set; } = ProductStatus.Available;

        public bool IsActive { get; set; } = true;
    }
}
