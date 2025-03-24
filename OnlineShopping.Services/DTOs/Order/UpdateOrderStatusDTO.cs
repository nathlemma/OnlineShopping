using System.ComponentModel.DataAnnotations;
using OnlineShopping.Datahub.Models.Domain;

namespace OnlineShopping.Services.DTOs.Order
{
    public class UpdateOrderStatusDTO
    {
        [Required(ErrorMessage = "Status is required")]
        public OrderStatus Status { get; set; }
    }
}