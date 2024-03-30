using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Project.ViewModels // Pas deze namespace aan op basis van de structuur van je project
{
    public class AdminPanelViewModel
    {
        public List<IdentityUser> Users { get; set; }
        public List<IdentityUser> Admins { get; set; }
    }
}
