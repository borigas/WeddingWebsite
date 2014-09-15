using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WeddingWebsite.Models
{
    public class WeddingWebsiteContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public WeddingWebsiteContext() : base("name=WeddingWebsiteContext")
        {
        }

        public System.Data.Entity.DbSet<WeddingWebsite.Models.Rsvp> Rsvps { get; set; }


        internal static void Seed()
        {
            WeddingWebsiteContext context = new WeddingWebsiteContext();

            context.Database.CreateIfNotExists();

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
