using System.Collections.Generic;

namespace Project.Models
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public string UserEmail { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; } // Property for holding order items
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
