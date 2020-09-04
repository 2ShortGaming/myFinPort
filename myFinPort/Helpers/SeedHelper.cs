using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using myFinPort.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace myFinPort.Helpers
{
    public class SeedHelper
    {
        private ApplicationDbContext db { get; set; }

        public SeedHelper(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        public void SeedRoles(RoleManager<IdentityRole> rm)
        {
            //  Admin role
            if (!db.Roles.Any(r => r.Name == "Admin"))
            {
                rm.Create(new IdentityRole { Name = "Admin" });
            }

            //  Project Manager role
            if (!db.Roles.Any(r => r.Name == "Head"))
            {
                rm.Create(new IdentityRole { Name = "Head" });
            }

            // Developer role
            if (!db.Roles.Any(r => r.Name == "Member"))
            {
                rm.Create(new IdentityRole { Name = "Member" });
            }

            // Submitter role
            if (!db.Roles.Any(r => r.Name == "New User"))
            {
                rm.Create(new IdentityRole { Name = "New User" });
            }

            db.SaveChanges();
        }

        // May not need this, or it has to be more robust.
        // reason?  Because when someone registers, they must create a household (is that right?)
        public void SeedDemoUsers(UserManager<ApplicationUser> um)
        {
            string demoEmail = "";
            string demoPassword = "";

            // set up a demo administrator
            demoEmail = WebConfigurationManager.AppSettings["AdminDemoEmail"];
            demoPassword = WebConfigurationManager.AppSettings["DemoPassword"];
            var avatar = WebConfigurationManager.AppSettings["DefaultAvatarPath"];

            if (!db.Users.Any(u => u.Email == demoEmail))
            {
                um.Create(new ApplicationUser()
                {
                    Email = demoEmail,
                    UserName = demoEmail,
                    FirstName = "Demo",
                    LastName = "Admin",
                    AvatarPath = avatar
                }, demoPassword);

                // grab the Id that just created by adding the above user
                var userId = um.FindByEmail(demoEmail).Id;

                // now that I have created a user I want to assign the person to the specific role
                um.AddToRole(userId, "Admin");
            }

            // set up a demo project manager
            demoEmail = WebConfigurationManager.AppSettings["HeadDemoEmail"];
            if (!db.Users.Any(u => u.Email == demoEmail))
            {
                um.Create(new ApplicationUser()
                {
                    Email = demoEmail,
                    UserName = demoEmail,
                    FirstName = "Demo",
                    LastName = "Head",
                    AvatarPath = avatar
                }, demoPassword);

                // grab the Id that just created by adding the above user
                var userId = um.FindByEmail(demoEmail).Id;

                // now that I have created a user I want to assign the person to the specific role
                um.AddToRole(userId, "Head");
            }

            // set up a demo developer
            demoEmail = WebConfigurationManager.AppSettings["MemberDemoEmail"];
            if (!db.Users.Any(u => u.Email == demoEmail))
            {
                um.Create(new ApplicationUser()
                {
                    Email = demoEmail,
                    UserName = demoEmail,
                    FirstName = "Demo",
                    LastName = "Member",
                    AvatarPath = avatar
                }, demoPassword);

                // grab the Id that just created by adding the above user
                var userId = um.FindByEmail(demoEmail).Id;

                // now that I have created a user I want to assign the person to the specific role
                um.AddToRole(userId, "Member");
            }

            db.SaveChanges();
        }



    }
}