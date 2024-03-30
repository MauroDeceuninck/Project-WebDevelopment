using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Services;
using System.Diagnostics;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BreadPitContext _context;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, BreadPitContext context, ICartService cartService)
        {
            _logger = logger;
            _context = context;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var cartItems = _cartService.GetCartItems();
            ViewBag.CartItems = cartItems;

            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}