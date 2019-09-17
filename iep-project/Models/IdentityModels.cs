using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace iep_project.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        [DisplayName("Number of tokens")]
        public int NumberOfTokens { get; set; } = 0;
        public bool Active { get; set; } = true;

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // modelBuilder.Entity<Answer>().HasOptional(a => a.ParentAnswer).WithMany(a => a.ChildAnswers).HasForeignKey(a => a.AnswerId).WillCascadeOnDelete(true);
        }

        public System.Data.Entity.DbSet<iep_project.Models.Question> Questions { get; set; }

        public System.Data.Entity.DbSet<iep_project.Models.Category> Categories { get; set; }

        public System.Data.Entity.DbSet<iep_project.Models.Answer> Answers { get; set; }

        public System.Data.Entity.DbSet<iep_project.Models.Offer> Offers { get; set; }

        public System.Data.Entity.DbSet<iep_project.Models.Price> Prices { get; set; }

        public System.Data.Entity.DbSet<iep_project.Models.Order> Orders { get; set; }
    }
}