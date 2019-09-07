namespace iep_project.Migrations
{
    using iep_project.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Security.Claims;

    

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            this.SeedRoles(context);

            this.SeedUsers(context);

            this.SeedCategories(context);
        }

        private void SeedRoles(ApplicationDbContext context)
        {
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            string[] roles = { "Admin", "User", "Agent" };

            foreach (string role in roles)
            {
                if (!roleManager.RoleExists(role)) roleManager.Create(new IdentityRole { Name = role });
            }
        }

        private class ApplicationUserSeed
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            public string Role { get; set; }
        }

        private void SeedUsers(ApplicationDbContext context)
        {
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            ApplicationUserSeed[] userSeeds = {
                new ApplicationUserSeed {
                    Email = "kosta.bizetic@gmail.com",
                    Password = "Pass123$",
                    Name = "Kosta",
                    Role = "Admin" },
                new ApplicationUserSeed {
                    Email = "u@u.com",
                    Password = "Pass123$",
                    Name = "User",
                    Role = "User" },
                new ApplicationUserSeed {
                    Email = "a@a.com",
                    Password = "Pass123$",
                    Name = "Agent",
                    Role = "Agent" }
            };

            foreach (ApplicationUserSeed userSeed in userSeeds)
            {
                if (!context.Users.Any(u => u.UserName == userSeed.Email))
                {
                    var user = new ApplicationUser { UserName = userSeed.Email, Email = userSeed.Email, Name = userSeed.Name };
                    userManager.Create(user, userSeed.Password);
                    userManager.AddToRole(user.Id, userSeed.Role);
                };
            }
        }

        private void SeedCategories(ApplicationDbContext context)
        {
            Category[] categories = new Category[] {
                new Category { CategoryName = "Category1" },
                new Category { CategoryName = "Category2" },
                new Category { CategoryName = "Category4" }};

            foreach (Category category in categories)
            {
                if (!context.Categories.Any(c => c.CategoryName == category.CategoryName))
                {
                    context.Categories.Add(category);
                }
            }
        }
    }
}
