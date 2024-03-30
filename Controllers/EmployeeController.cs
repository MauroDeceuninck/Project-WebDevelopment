using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class EmployeeController : Controller
    {
        private readonly BreadPitContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EmployeeController(BreadPitContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders.ToListAsync();
            var orderDetailsList = new List<OrderDetailsViewModel>();

            foreach (var order in orders)
            {
                var user = await _userManager.FindByIdAsync(order.UserId);
                var userEmail = user?.Email ?? "Unknown";

                orderDetailsList.Add(new OrderDetailsViewModel
                {
                    OrderId = order.Id,
                    UserEmail = userEmail,
                    OrderItems = _context.OrderItems
                        .Where(oi => oi.OrderId == order.Id)
                        .Select(oi => new OrderItemViewModel
                        {
                            ProductName = oi.Product.Name,
                            Quantity = oi.Quantity,
                            Price = oi.Price
                        })
                        .ToList()
                });
            }

            // Calculate total price for each order and store it in a dictionary
            var totalPriceDict = new Dictionary<int, decimal>();
            foreach (var orderDetails in orderDetailsList)
            {
                var totalPrice = orderDetails.OrderItems.Sum(item => item.Quantity * item.Price);
                totalPriceDict.Add(orderDetails.OrderId, totalPrice);
            }

            ViewBag.TotalPriceDict = totalPriceDict;

            return View(orderDetailsList);
        }
    }
}
