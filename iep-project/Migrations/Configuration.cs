namespace iep_project.Migrations
{
    using iep_project.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<iep_project.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(iep_project.Models.ApplicationDbContext context)
        {
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            string[] roles = { "Admin", "User", "Agent" };

            foreach (string role in roles)
            {
                if (!roleManager.RoleExists(role)) roleManager.Create(new IdentityRole { Name = role });
            }

                          
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            string email = "kosta.bizetic@gmail.com";
            string password = "Pass123$";
            var hash = new PasswordHasher();
            var user = new ApplicationUser { UserName = email, Email = email, Name = "Kosta"};
            if (!context.Users.Any(u => u.UserName == email))
            {
                userManager.Create(user, password);
                userManager.AddToRole(user.Id, "Admin");
            }    
        }
    }
}
