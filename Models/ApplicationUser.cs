using Microsoft.AspNetCore.Identity;
using Project.Models;

namespace Project.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Additional properties can be added as needed

        // You can add your custom methods or properties here as well
    }
}