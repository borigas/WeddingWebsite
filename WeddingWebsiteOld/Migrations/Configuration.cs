namespace WeddingWebsite.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity.EntityFramework;
    using WeddingWebsite.Models;
    using Microsoft.AspNet.Identity;

    internal sealed class Configuration : DbMigrationsConfiguration<WeddingWebsite.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        // Based on example from http://www.typecastexception.com/post/2013/10/27/Configuring-Db-Connection-and-Code-First-Migration-for-Identity-Accounts-in-ASPNET-MVC-5-and-Visual-Studio-2013.aspx#Seeding-the-Database-with-an-Initial-User-Records
        protected override void Seed(WeddingWebsite.Models.ApplicationDbContext context)
        {
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
