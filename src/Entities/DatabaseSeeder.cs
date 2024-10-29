using Check_Inn.DAL;
using Check_Inn.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Check_Inn.Entities
{
    public class DatabaseSeeder
    {
        private readonly CheckInnContext _context;
        private readonly CheckInnUserManager _userManager;
        private readonly CheckInnRoleManager _roleManager;

        public DatabaseSeeder(CheckInnContext context, CheckInnUserManager userManager, CheckInnRoleManager roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedAdminUserAsync();
        }

        private async Task SeedRolesAsync()
        {
            var roles = new[] { "Admin", "AccomodationManager", "RestaurantManager", "HotelManager" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private async Task SeedAdminUserAsync()
        {
            const string adminEmail = "alexbanut10@gmail.com";
            const string adminPassword = "Alex123!";

            var user = await _userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                user = new User { UserName = adminEmail, Email = adminEmail, FullName = "Alex Banut" };
                var result = await _userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user.Id, "Admin");
                }
            }
        }
    }
}