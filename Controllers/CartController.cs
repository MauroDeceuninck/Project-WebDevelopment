using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Project.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BreadPitContext _dbContext;

        public CartController(IHttpContextAccessor httpContextAccessor, BreadPitContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var cartItems = GetCartItems();
            return View(cartItems);
        }


        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.ProductId == productId);

            if (product == null)
            {
                return NotFound();
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

            //// Update total price for each item
            //foreach (var cartItem in cartItems)
            //{
            //    cartItem.TotalPrice = cartItem.Product.Price * cartItem.Quantity;
            //}

            SaveCartItems(cartItems);

            return RedirectToAction("Index");
        }




        private List<CartItem> GetCartItems()
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
    }
}
