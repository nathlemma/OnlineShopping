using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Services.DTOs.Customer
{
    public class AddCustomerDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Phone number must be between 10-15 digits")]
        public string PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "Address is required")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }
    }
}