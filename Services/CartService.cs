using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Project.Services
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BreadPitContext _dbContext;

        public CartService(IHttpContextAccessor httpContextAccessor, BreadPitContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public void AddToCart(int productId, int quantity)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.ProductId == productId);

            if (product == null)
            {
                // Handle error or throw exception
                return;
            }

            var cartItems = GetCartItems();

            var existingCartItem = cartItems.FirstOrDefault(ci => ci.Product.ProductId == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    Product = product,
                    Quantity = quantity
                };

                cartItems.Add(cartItem);
            }

            SaveCartItems(cartItems);
        }

        public void RemoveFromCart(int productId)
        {
            var cartItems = GetCartItems();
            var cartItemToRemove = cartItems.FirstOrDefault(ci => ci.Product.ProductId == productId);
            if (cartItemToRemove != null)
            {
                cartItems.Remove(cartItemToRemove);
                SaveCartItems(cartItems);
            }
        }


        public List<CartItem> GetCartItems()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartItemsJson = session.GetString("CartItems");
            return cartItemsJson == null ? new List<CartItem>() :
                JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson);
        }

        private void SaveCartItems(List<CartItem> cartItems)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString("CartItems", JsonSerializer.Serialize(cartItems));
        }

        public decimal GetTotalAmount()
        {
            var cartItems = GetCartItems();
            decimal totalAmount = 0;
            foreach (var cartItem in cartItems)
            {
                totalAmount += cartItem.Product.Price * cartItem.Quantity;
            }
            return totalAmount;
        }

        public void ClearCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.Remove("CartItems");
        }

    }
}
