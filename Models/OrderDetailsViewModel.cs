using System.Collections.Generic;

namespace Project.Models
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; }
        public int NewProductId { get; set; }

        public int NewQuantity { get; set; }
    }

    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }


}
