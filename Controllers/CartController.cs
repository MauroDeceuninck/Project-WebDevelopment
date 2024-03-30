using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Services;

namespace Project.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            var cartItems = _cartService.GetCartItems();
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            _cartService.AddToCart(productId, quantity);
            var cartItems = _cartService.GetCartItems();
            return PartialView("_CartItemsPartial", cartItems);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            _cartService.RemoveFromCart(productId);
            var cartItems = _cartService.GetCartItems();
            return PartialView("_CartItemsPartial", cartItems);
        }



        public IActionResult TotalAmount()
        {
            var totalAmount = _cartService.GetTotalAmount();
            return Content(totalAmount.ToString());
        }

    }
}
