using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Services.DTOs.Product
{
    public class UpdateProductDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }
        
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 9999999.99, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int StockQuantity { get; set; }
    }
}