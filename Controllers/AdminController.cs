using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly BreadPitContext _context;
    private readonly IdentityBreadPitContext _identityBreadPitContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(BreadPitContext context, IdentityBreadPitContext identityBreadPitContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _identityBreadPitContext = identityBreadPitContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> AddToAdminRole(string userId)
    {
        // Find the user by their unique identifier
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            // Handle user not found
            return NotFound();
        }

        // Check if the admin role exists, if not, create it
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Assign the user to the admin role
        await _userManager.AddToRoleAsync(user, "Admin");

        // Optionally, you can check if the user is now in the admin role
        var isInAdminRole = await _userManager.IsInRoleAsync(user, "Admin");

        // Redirect or return a success message
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Products()
    {
        var products = _context.Products.ToList();
        return View(products);
    }

    [HttpGet]
    public IActionResult ProductCreate()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ProductCreate(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Products");
        }
        return View(product);
    }

    public async Task<IActionResult> Orders()
    {
        var orders = _context.Orders.ToList();
        var orderDetailsList = new List<OrderDetailsViewModel>();

        foreach (var order in orders)
        {
            var user = await _userManager.FindByIdAsync(order.UserId);
            var userEmail = user?.Email ?? "Unknown";

            // Populate OrderItems for each order
            var orderItems = _context.OrderItems
                .Where(oi => oi.OrderId == order.Id)
                .Select(oi => new OrderItemViewModel
                {
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                })
                .ToList();

            orderDetailsList.Add(new OrderDetailsViewModel
            {
                OrderId = order.Id,
                UserEmail = userEmail,
                OrderItems = orderItems
            });
        }

        return View(orderDetailsList);
    }



    public IActionResult OrderEdit(int id)
    {
        var order = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        // Fetch available products
        var products = _context.Products.ToList();

        // Create a SelectList for the dropdown
        ViewBag.ProductList = new SelectList(products, "ProductId", "Name");

        return View(order);
    }



    [HttpPost]
    public IActionResult OrderEdit(Order order, string action)
    {
        if (action == "SaveChanges")
        {
            Console.WriteLine("Inside SaveChanges");
            // Handle updating order item quantities
            if (ModelState.IsValid)
            {
                Console.WriteLine(ModelState);
                Console.WriteLine("Inside SaveChanges ModelState.IsValid");
                foreach (var item in order.OrderItems)
                {
                    // Find the order item in the database
                    var orderItem = _context.OrderItems.FirstOrDefault(oi => oi.Id == item.Id);
                    Console.WriteLine("Inside Foreach");
                    // If the order item exists, update its quantity
                    if (orderItem != null)
                    {
                        Console.WriteLine("Inside If");
                        Console.WriteLine(item.Quantity);
                        orderItem.Quantity = item.Quantity;
                    }
                    else
                    {
                        // Log an error if the order item is not found
                        Console.WriteLine($"Order item with ID {item.Id} not found.");
                    }
                }

                // Save changes to the database
                _context.SaveChanges();
                Console.WriteLine("Order items updated successfully!");
            }
            else
            {
                // Log model state errors if the model state is invalid
                Console.WriteLine("ModelState is not valid!");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
            }
        }
        else if (action == "AddProduct")
        {
            // Handle adding product to order
            return AddProduct(order);
        }

        // Redirect to the Orders page
        return RedirectToAction("Orders");
    }

    [HttpPost]
    public IActionResult UpdateOrder(Order order)
    {
        // Update order items
        if (ModelState.IsValid)
        {
            foreach (var item in order.OrderItems)
            {
                var orderItem = _context.OrderItems.FirstOrDefault(oi => oi.Id == item.Id);
                if (orderItem != null)
                {
                    orderItem.Quantity = item.Quantity;
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Orders");
        }
        return View("OrderEdit", order);
    }


    private IActionResult AddProduct(Order order)
    {
        Console.WriteLine("Inside AddProduct--------------------------------------------------------------------------------------------------------------------");
        if (ModelState.IsValid)
        {
            Console.WriteLine("Inside AddProduct ModelState.IsValid--------------------------------------------------------------------------------------------------------------------");
            // Retrieve the order from the database
            var existingOrder = _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == order.Id);

            if (existingOrder != null)
            {
                Console.WriteLine("Inside AddProduct existingOrder != null--------------------------------------------------------------------------------------------------------------------");
                // Find the selected product by its id
                var product = _context.Products.FirstOrDefault(p => p.ProductId == order.NewProductId);

                if (product != null)
                {
                    Console.WriteLine("Inside AddProduct product != null--------------------------------------------------------------------------------------------------------------------");
                    // Add the selected product with the specified quantity to the order
                    existingOrder.OrderItems.Add(new OrderItem
                    {
                        ProductId = product.ProductId,
                        Quantity = order.NewQuantity,
                        Price = product.Price
                    });

                    // Set the UserId property of the order
                    // You can retrieve the current user's id using HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    existingOrder.UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    // Save changes to the database
                    _context.SaveChanges();
                    Console.WriteLine("Product added successfully!");
                }
                else
                {
                    Console.WriteLine("Product not found!");
                }
            }
            else
            {
                Console.WriteLine("Existing order not found!");
            }
        }
        else
        {
            Console.WriteLine("ModelState is not valid!");
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
        }

        // Redirect back to the OrderEdit page
        return RedirectToAction("OrderEdit", new { id = order.Id });
    }






    [HttpPost]
    public IActionResult OrderDelete(int id)
    {
        var order = _context.Orders.Find(id);
        if (order == null)
        {
            return NotFound();
        }

        _context.Orders.Remove(order);
        _context.SaveChanges();
        return RedirectToAction("Orders");
    }

    [HttpPost]
    public IActionResult DeleteBulk(int[] selectedOrders)
    {
        if (selectedOrders != null && selectedOrders.Length > 0)
        {
            var ordersToDelete = _context.Orders.Where(o => selectedOrders.Contains(o.Id)).ToList();
            _context.Orders.RemoveRange(ordersToDelete);
            _context.SaveChanges();
        }
        return RedirectToAction("Orders");
    }

    [HttpGet]
    public IActionResult ProductEdit()
    {
        var products = _context.Products.ToList();
        return View(products);
    }

    [HttpPost]
    public IActionResult UpdatePrices(Dictionary<int, decimal> productPrices)
    {
        if (productPrices != null && productPrices.Count > 0)
        {
            foreach (var kvp in productPrices)
            {
                var product = _context.Products.Find(kvp.Key);
                if (product != null)
                {
                    product.Price = kvp.Value;
                }
            }
            _context.SaveChanges();
        }
        return RedirectToAction("Products");
    }

    public async Task<IActionResult> AdminPanel()
    {
        // Haal alle gebruikers op uit de database
        var allUsers = _identityBreadPitContext.Users.ToList();

        // Haal alle beheerders op uit de database
        var adminUsers = await GetAdminUsersAsync();

        var model = new AdminPanelViewModel
        {
            Users = allUsers,
            Admins = adminUsers
        };

        return View(model);
    }

    private async Task<List<IdentityUser>> GetAdminUsersAsync()
    {
        var allUsers = await _identityBreadPitContext.Users.ToListAsync();
        var adminUsers = new List<IdentityUser>();

        foreach (var user in allUsers)
        {
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                adminUsers.Add(user);
            }
        }

        return adminUsers;
    }




    [HttpPost]
    public async Task<IActionResult> AddAdmin(string userEmail)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user != null)
        {
            await _userManager.AddToRoleAsync(user, "Admin");
            // Handle success
            Console.WriteLine("User Added to admin role!", userEmail, "User:", user);
        }
        else
        {
            // Handle user not found
            Console.WriteLine("user nor found!", userEmail);
        }
        return RedirectToAction("AdminPanel");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveAdmin(string userEmail)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user != null)
        {
            await _userManager.RemoveFromRoleAsync(user, "Admin");
            // Handle success
        }
        else
        {
            // Handle user not found
        }
        return RedirectToAction("AdminPanel");
    }

    [HttpPost]
    public async Task<IActionResult> ChangeRole(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            // Eerst de gebruiker verwijderen uit alle rollen
            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            // Toevoegen van de nieuwe rol
            await _userManager.AddToRoleAsync(user, newRole);
        }
        return RedirectToAction("AdminPanel");
    }

    // Add other action methods for updating and deleting products
}
