using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using myFinPort.Extensions;
using myFinPort.Helpers;
using myFinPort.Models;
using myFinPort.ViewModels;

namespace myFinPort.Controllers
{
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserRolesHelper rolesHelper = new UserRolesHelper();
        HouseholdHelper hhHelper = new HouseholdHelper();


        // GET: Households
        public ActionResult Index()
        {
            return View(db.Households.ToList());
        }

        // GET: Members
        public ActionResult Members()
        {
            return View(hhHelper.ListHouseholdMembers());
        }

        // GET: Households/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }


        // GET: Households/BuildHouse
        public ActionResult BuildHouse()
        {
            var model = new BuildHouseWizardVM();
            return View(model);
        }

        // POST: Households/BuildHouse
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "New User")]
        public async Task<ActionResult> BuildHouse(BuildHouseWizardVM model, bool isPersonalAccount = false)
        {
            if (ModelState.IsValid)
            {
                // success path
                db.Households.Add(model.Household);
                db.SaveChanges();

                var user = db.Users.Find(User.Identity.GetUserId());
                user.HouseholdId = model.Household.Id;
                rolesHelper.UpdateUserRole(user.Id, "Head");
                db.SaveChanges();

                await AuthorizeExtensions.RefreshAuthentication(HttpContext, user);

                // add bank account info
                var bankAccount = new BankAccount
                    (
                        model.StartingBalance,
                        model.BankAccount.WarningBalance,
                        model.BankAccount.AccountName
                    );

                bankAccount.HouseholdId = (int)user.HouseholdId;
                bankAccount.AccountType = model.BankAccount.AccountType;

                if (isPersonalAccount)
                {
                    bankAccount.OwnerId = user.Id;
                }
                else
                {
                    bankAccount.OwnerId = null;
                }

                db.BankAccounts.Add(bankAccount);

                // add budget info
                var budget = new Budget();
                budget.HouseholdId = (int)model.Household.Id;
                budget.BudgetName = model.Budget.BudgetName;
                db.Budgets.Add(budget);
                db.SaveChanges();

                // add budget item info
                var budgetItem = new BudgetItem();
                budgetItem.BudgetId = budget.Id;
                budgetItem.TargetAmount = model.BudgetItem.TargetAmount;
                budgetItem.ItemName = model.BudgetItem.ItemName;
                db.BudgetItems.Add(budgetItem);
                db.SaveChanges();

                // now that the household has been established, refresh their login and send them to the dashboard.
                
                return RedirectToAction("Dashboard", "Home");
            }

            // error
            return View(model);
        }


        // GET: Households/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "New User")]
        public async Task<ActionResult> Create([Bind(Include = "Id,HouseholdName,Greeting")] Household household)
        {
            if (ModelState.IsValid)
            {
                household.Created = DateTime.Now;  // this may come out later
                db.Households.Add(household);
                db.SaveChanges();

                var user = db.Users.Find(User.Identity.GetUserId());
                user.HouseholdId = household.Id;
                rolesHelper.UpdateUserRole(user.Id, "Head");
                db.SaveChanges();

                await AuthorizeExtensions.RefreshAuthentication(HttpContext, user);

                return RedirectToAction("ConfigureHouse");
            }

            return View(household);
        }

        [HttpGet]
        //[Authorize(Roles = "Head")]
        public ActionResult ConfigureHouse()
        {
            var model = new ConfigureHouseVM();
            model.HouseholdId = User.Identity.GetHouseholdId();
            if(model.HouseholdId == null)
            {
                // this is the fail case
                return RedirectToAction("Create");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfigureHouse(ConfigureHouseVM model)
        {
            // Create the bank account(s)
            var bankAccount = new BankAccount
                (
                    model.StartingBalance,
                    model.BankAccount.WarningBalance,
                    model.BankAccount.AccountName
                );

            bankAccount.AccountType = model.BankAccount.AccountType;
            db.BankAccounts.Add(bankAccount);

            var budget = new Budget();
            budget.HouseholdId = (int)model.HouseholdId;
            budget.BudgetName = model.Budget.BudgetName;
            db.Budgets.Add(budget);
            db.SaveChanges();

            var budgetItem = new BudgetItem();
            budgetItem.BudgetId = budget.Id;
            budgetItem.TargetAmount = model.BudgetItem.TargetAmount;
            budgetItem.ItemName = model.BudgetItem.ItemName;
            db.BudgetItems.Add(budgetItem);

            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }




        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdName,Greeting,Created,IsDeleted")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }

        // what do we need to do to leave a household?
        // user leaving the house has HHId set to null
        // if the user is in the Head role someone must take over
        // if the user is in the Member role they can just leave
        // anyone leaving needs role reset to New User
        // if the user is the last person in the household the household can be deleted.
        // async Task == void return type
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Head, Member")]
        public async Task<ActionResult> LeaveAsync()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var role = rolesHelper.ListUserRoles(userId).FirstOrDefault();

            switch (role)
            {
                case "Head":
                    // -1 because I don't count the user doing this
                    var memberCount = db.Users.Where(u => u.HouseholdId == user.HouseholdId).Count() - 1;
                    if(memberCount >= 1)
                    {
                        TempData["Message"] = $"You are unable to leave the Household!  There are still <b>{memberCount}</b> other members in the household.  You must select one of them to assume your role!";
                        // TODO: must create an "ExitDenied" action in this controller to deal with this next line.
                        return RedirectToAction("ExitDenied");
                    }

                    // this is a "soft" delete.  record stays in the database, but you can limit access on the front end
                    user.Household.IsDeleted = true;

                    // uncomment the next two lines for a hard delete, the record is removed from the database and anything with the
                    // household foreign key will be cascade deleted
                    //var household = db.Households.Find(user.HouseholdId);
                    //db.Households.Remove(household);

                    user.HouseholdId = null;
                    db.SaveChanges();

                    rolesHelper.UpdateUserRole(userId, "New User");
                    await AuthorizeExtensions.RefreshAuthentication(HttpContext, user);
                    return RedirectToAction("Dashboard", "Home");

                case "Member":
                    user.HouseholdId = null;
                    db.SaveChanges();

                    rolesHelper.UpdateUserRole(userId, "New User");
                    await AuthorizeExtensions.RefreshAuthentication(HttpContext, user);
                    return RedirectToAction("Dashboard", "Home");

                default:
                    return RedirectToAction("Dashboard", "Home");
            }
        }

        [Authorize(Roles = "Head")]
        public ActionResult ExitDenied()
        {
            return View();
        }

        [Authorize(Roles = "Head")]
        public ActionResult ChangeHead()
        {
            // TODO: look up null coelessing operator
            var myHouseId = User.Identity.GetHouseholdId();

            if (myHouseId == 0)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            var members = db.Users.Where(u => u.HouseholdId == myHouseId).ToList();
            ViewBag.NewHoH = new SelectList(members, "Id", "FullName");

            return View();
        }

        [HttpPost, ActionName("ChangeHead")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeHeadAsync(string newHoH, bool leave)
        {
            if (string.IsNullOrEmpty(newHoH))
            {
                return RedirectToAction("Dashboard", "Home");
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            rolesHelper.UpdateUserRole(newHoH, "Head");
            if (leave)
            {
                // TODO: He copied stuff and I couldn't keep up.
            }

            user.HouseholdId = null;
            db.SaveChanges();

            rolesHelper.UpdateUserRole(User.Identity.GetUserId(), "New User");
            await AuthorizeExtensions.RefreshAuthentication(HttpContext, user);
            return RedirectToAction("Dashboard", "Home");
        }


        // GET: Households/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Households.Find(id);
            db.Households.Remove(household);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
