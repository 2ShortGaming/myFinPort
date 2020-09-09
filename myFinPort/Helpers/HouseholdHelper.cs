using myFinPort.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myFinPort.Helpers
{
    public class HouseholdHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int HouseholdMembersCount()
        {
            return db.Users.ToList().Count;
        }

        public List<ApplicationUser> ListHouseholdMembers()
        {
            return db.Users.ToList();
        }

    }
}