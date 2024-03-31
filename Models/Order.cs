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

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        [Display(Name = "Product")]
        public int NewProductId { get; set; }

        [Display(Name = "Quantity")]
        public int NewQuantity { get; set; }
    }

    public class OrderItem
    {

        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}
