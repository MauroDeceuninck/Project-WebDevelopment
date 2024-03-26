namespace Project.Models
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get { return Product.Price * Quantity; } }
    }
}
