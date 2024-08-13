using ProdajaAutomobila.Models;
using Microsoft.AspNetCore.Identity;

namespace ProdajaAutomobila.Services
{
    public class DatabaseInitializer
    {
        public static async Task SeedDataAsync(UserManager<ApplicationUser>? userManager,
            RoleManager<IdentityRole>? roleManager)
        {
            if (userManager == null || roleManager == null)
            {
                Console.WriteLine("userManager or roleManager is null => exit");
                return;
            }

            // check if we have the admin role or not
            var exists = await roleManager.RoleExistsAsync("admin");
            if (!exists)
            {
                Console.WriteLine("Admin ne postoji i biti će kreiran");
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }

            // check if we have the seller role or not
            exists = await roleManager.RoleExistsAsync("Prodavač");
            if (!exists)
            {
                Console.WriteLine("Prodavač ne postoji i biti će kreiran");
                await roleManager.CreateAsync(new IdentityRole("seller"));
            }


            // check if we have the client role or not
            exists = await roleManager.RoleExistsAsync("Klijent");
            if (!exists)
            {
                Console.WriteLine("Klijent ne postoji i biti će kreiran");
                await roleManager.CreateAsync(new IdentityRole("client"));
            }


            // check if we have at least one admin user or not
            var adminUsers = await userManager.GetUsersInRoleAsync("admin");
            if (adminUsers.Any())
            {
                // Admin user already exists => exit
                Console.WriteLine("Admin već postoji => exit");
                return;
            }


            // create the admin user
            var user = new ApplicationUser()
            {
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "admin@admin.com", // UserName will be used to authenticate the user
                Email = "admin@admin.com",
                CreatedAt = DateTime.Now,
            };

            string initialPassword = "admin123";


            var result = await userManager.CreateAsync(user, initialPassword);
            if (result.Succeeded)
            {
                // set the user role
                await userManager.AddToRoleAsync(user, "admin");
                Console.WriteLine("Admin je kreiran uspješno! Molimo Vas unesite novi password!");
                Console.WriteLine("Email: " + user.Email);
                Console.WriteLine("Password: " + initialPassword);
            }
        }
    }
}
