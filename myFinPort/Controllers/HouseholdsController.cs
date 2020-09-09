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
