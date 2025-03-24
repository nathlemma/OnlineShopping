using System;
using System.Collections.Generic;

namespace OnlineShopping.Datahub.Models.Domain
{
    public class Customer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Order>? Orders { get; set; }
    }
}