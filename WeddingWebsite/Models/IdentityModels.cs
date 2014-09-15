using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WeddingWebsite.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public string Name { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("WeddingWebsiteContext", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<WeddingWebsite.Models.Rsvp> Rsvps { get; set; }

        internal static void Seed()
        {
            ApplicationDbContext context = ApplicationDbContext.Create();

            string[] adminEmails = new string[] { "borigas@gmail.com" };
            string[] roles = new string[] { "admin", "user" };

            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            foreach (string role in roles)
            {
                if (roleManager.FindByName(role) == null)
                {
                    roleManager.Create(new IdentityRole() { Name = role });
                }
            }

            var userManager = new ApplicationUserManager(
                new UserStore<ApplicationUser>(context));

            foreach (string email in adminEmails)
            {
                if (userManager.FindByEmail(email) == null)
                {
                    var user = new ApplicationUser()
                    {
                        UserName = email,
                        Email = email,
                        Name = "Seed Admin",
                    };
                    userManager.Create(user, "changeme");

                    userManager.AddToRole(user.Id, "admin");
                }
            }
        }
    }
}