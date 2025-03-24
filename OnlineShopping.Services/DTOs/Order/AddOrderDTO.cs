using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Services.DTOs.Order
{
    public class AddOrderDTO
    {
        [Required(ErrorMessage = "Customer ID is required")]
        public int CustomerId { get; set; }
        
        [Required(ErrorMessage = "Order items are required")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item")]
        public List<AddOrderItemDTO> OrderItems { get; set; }
    }
}