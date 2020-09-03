using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using myFinPort.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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


    }
}