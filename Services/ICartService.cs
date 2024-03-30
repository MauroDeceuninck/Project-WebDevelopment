using Project.Models;
using System.Collections.Generic;

namespace Project.Services
{
    public interface ICartService
    {
        List<CartItem> GetCartItems();
        void AddToCart(int productId, int quantity);
        void RemoveFromCart(int productId);
        decimal GetTotalAmount();
        void ClearCart();
    }
}
