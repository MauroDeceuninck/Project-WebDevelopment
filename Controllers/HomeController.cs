﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Diagnostics;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BreadPitContext _context;

        public HomeController(ILogger<HomeController> logger, BreadPitContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
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

        //     private List<Product> GetProducts()
        //     {
        //         return new List<Product>
        //         {
        //             new Product { ProductId = 1, Name = "Broodje A", Price = 5.00M, ImageUrl = "https://www.leukerecepten.nl/wp-content/uploads/2021/05/broodje-gezond-v.jpg" },
        //             new Product { ProductId = 2, Name = "Broodje B", Price = 6.50M, ImageUrl = "data:image/jpeg;base64,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" },
        //             new Product { ProductId = 3, Name = "Broodje C", Price = 5.00M, ImageUrl = "https://www.leukerecepten.nl/wp-content/uploads/2021/05/broodje-gezond-v.jpg" },
        //             new Product { ProductId = 4, Name = "Broodje D", Price = 5.00M, ImageUrl = "https://www.leukerecepten.nl/wp-content/uploads/2021/05/broodje-gezond-v.jpg" },
        //             new Product { ProductId = 5, Name = "Broodje E", Price = 5.00M, ImageUrl = "https://www.leukerecepten.nl/wp-content/uploads/2021/05/broodje-gezond-v.jpg" },
        //	// Add other products
        //};
        //     }

        //     public ActionResult Index()
        //     {
        //         var products = GetProducts();
        //         return View(products);
        //     }
    }
}