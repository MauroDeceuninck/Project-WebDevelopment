using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }


}
