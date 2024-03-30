using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Data;
using Project.Models;
using Project.Services;
using System;
using System.Linq;
using System.Security.Claims;

namespace Project.Controllers
{
    public class OrderController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly BreadPitContext _dbContext;

        public OrderController(ICartService cartService, UserManager<IdentityUser> userManager, BreadPitContext dbContext)
        {
            _cartService = cartService;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public IActionResult PlaceOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's Id

            Console.WriteLine(userId);

            var cartItems = _cartService.GetCartItems();
            if (cartItems.Count == 0)
            {
                TempData["Message"] = "Your shopping cart is empty. Please add items before placing an order.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Create a new order
                var order = new Order
                {
                    UserId = userId, // Set the UserId to associate the order with the current user
                    OrderItems = cartItems.Select(item => new OrderItem
                    {
                        ProductId = item.Product.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Product.Price
                    }).ToList()
                };

                // Add order to the database
                _dbContext.Orders.Add(order);
                _dbContext.SaveChanges();

                // Clear the cart after successful order placement
                _cartService.ClearCart();

                TempData["Message"] = "Your order has been placed successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to place the order. Please try again later.";
                // Log the exception or handle it appropriately
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
