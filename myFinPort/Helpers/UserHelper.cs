using Microsoft.AspNet.Identity;
using myFinPort.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myFinPort.Helpers
{
    public class UserHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();

        public string GetFullName(string userId)
        {
            var user = db.Users.Find(userId);
            var firstName = user.FirstName;
            var lastName = user.LastName;
            return firstName + " " + lastName;
        }

        public string GetFullName()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            var firstName = user.FirstName;
            var lastName = user.LastName;
            return firstName + " " + lastName;
        }

        public string GetAvatarPath()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            return user.AvatarPath;
        }

        public string GetAvatarPath(string userId)
        {
            var user = db.Users.Find(userId);
            return user.AvatarPath;
        }

        public string GetEmail(string userId)
        {
            var user = db.Users.Find(userId);
            return user.Email;
        }

        public string GetEmail()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            return user.Email;
        }

        public string LastNameFirst(string userId)
        {
            var user = db.Users.Find(userId);
            return user.FullName;
        }

        public List<ApplicationUser> GetUserList()
        {
            return db.Users.ToList();
        }

        public List<ApplicationUser> GetUserList(string role)
        {
            return roleHelper.UsersInRole(role).ToList();
        }

        public bool CanEditUserInfo()
        {
            var loggedInUser = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());

            if (roleHelper.IsUserInRole(loggedInUser.Id, "Admin"))
            {
                return true;
            }
            return false;
        }

        public bool CanEditUserInfo(string userId)
        {
            var loggedInUser = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());

            if (roleHelper.IsUserInRole(userId, "Admin"))
            {
                return true;
            }
            else if (loggedInUser.Id == userId)
            {
                return true;
            }

            return false;
        }
    }
}