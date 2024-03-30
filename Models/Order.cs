using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // Collection of order items

        // New properties for adding products to the order
        [Display(Name = "Product")]
        public int NewProductId { get; set; } // Product ID for adding new products

        [Display(Name = "Quantity")]
        public int NewQuantity { get; set; } // Quantity for adding new products
    }

    public class OrderItem
    {

        public int Id { get; set; } // Primary key

        [ForeignKey("Order")]
        public int OrderId { get; set; } // Add OrderId property for the composite key

        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}
