using Microsoft.AspNetCore.Mvc;
using Project.Models;

public class OrderController : Controller
{
    // Simulate product data (in a real application, you would retrieve this from a database)
    private List<Product> GetProducts()
    {
        return new List<Product>
        {
            new Product { ProductId = 1, Name = "Broodje A", Price = 5.00M },
            new Product { ProductId = 2, Name = "Broodje B", Price = 6.50M },
            // Add other products
        };
    }

    public ActionResult PlaceOrder()
    {
        var products = GetProducts();
        ViewBag.Products = products;
        ViewBag.QuantityByProductId = new Dictionary<int, int>();

        return View();
    }

    [HttpPost]
    public ActionResult PlaceOrder(FormCollection form)
    {
        // Process the order, for example, save it to the database
        Dictionary<int, int> quantityByProductId = new Dictionary<int, int>();
        foreach (var key in form.Keys)
        {
            if (key.StartsWith("quantity_"))
            {
                int productId = int.Parse(key.Substring("quantity_".Length));
                int quantity = int.Parse(form[key]);
                quantityByProductId[productId] = quantity;
            }
        }

        // Perform the necessary actions with quantityByProductId

        return RedirectToAction("OrderSummary");
    }


    public ActionResult OrderSummary()
    {
        // Display an order summary (implement this as needed)
        return View();
    }
}
