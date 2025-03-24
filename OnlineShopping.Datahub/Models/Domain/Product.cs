using System;
using System.Collections.Generic;

namespace OnlineShopping.Datahub.Models.Domain
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}