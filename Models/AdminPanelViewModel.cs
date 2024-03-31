using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Project.ViewModels
{
    public class AdminPanelViewModel
    {
        public List<IdentityUser> Users { get; set; }
        public List<IdentityUser> Admins { get; set; }
    }
}
