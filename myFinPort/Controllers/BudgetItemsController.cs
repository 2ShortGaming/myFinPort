using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using myFinPort.Models;

namespace myFinPort.Controllers
{
    public class BudgetItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetItems
        public ActionResult Index()
        {
            var budgetItems = db.BudgetItems.Include(b => b.Budget);
            return View(budgetItems.ToList());
        }

        // GET: BudgetItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            return View(budgetItem);
        }

        // GET: BudgetItems/Create
        public ActionResult Create()
        {
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "OwnerId");
            return View();
        }

        // POST: BudgetItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BudgetId,Created,ItemName,TargetAmount,CurrentAmount,IsDeleted")] BudgetItem budgetItem)
        {
            if (ModelState.IsValid)
            {
                db.BudgetItems.Add(budgetItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "OwnerId", budgetItem.BudgetId);
            return View(budgetItem);
        }

        // Post: BudgetItems/NewBudgetItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Head, Member")]
        public ActionResult NewBudgetItem(string budgetItemName, decimal targetAmount, int Id)
        {
            var budgetItem = new BudgetItem();
            budgetItem.BudgetId = Id;
            budgetItem.TargetAmount = targetAmount;
            budgetItem.ItemName = budgetItemName;
            db.BudgetItems.Add(budgetItem);
            db.SaveChanges();

            return RedirectToAction("Details", "Budgets", new { Id = Id });
        }

        // GET: BudgetItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "OwnerId", budgetItem.BudgetId);
            return View(budgetItem);
        }

        // POST: BudgetItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BudgetId,Created,ItemName,TargetAmount,CurrentAmount,IsDeleted")] BudgetItem budgetItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budgetItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "OwnerId", budgetItem.BudgetId);
            return View(budgetItem);
        }

        // GET: BudgetItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            return View(budgetItem);
        }

        // POST: BudgetItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            db.BudgetItems.Remove(budgetItem);
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

        // PARTIAL VIEWS

        public PartialViewResult _EditBudgetItemModal(int id)
        {
            var model = db.BudgetItems.Find(id);

            return PartialView(model);
        }


        // POST: _EditBudgetItemModal/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _EditBudgetItemModal([Bind(Include = "Id,BudgetId,ItemName,TargetAmount,CurrentAmount")] BudgetItem model)
        {
            var budgetItem = db.BudgetItems.Find(model.Id);

            budgetItem.ItemName = model.ItemName;
            budgetItem.TargetAmount = model.TargetAmount;
            budgetItem.CurrentAmount = model.CurrentAmount;
            db.SaveChanges();

            return RedirectToAction("Details", "Budgets", new { Id = model.BudgetId });
        }

        public PartialViewResult _DeleteBudgetItemModal(int id)
        {
            var model = db.BudgetItems.Find(id);

            return PartialView(model);
        }


        // POST: _EditBudgetItemModal/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _DeleteBudgetItemModal([Bind(Include = "Id,BudgetId")] BudgetItem model)
        {
            var budgetItem = db.BudgetItems.Find(model.Id);

            budgetItem.IsDeleted = true;
            db.SaveChanges();

            return RedirectToAction("Details", "Budgets", new { Id = model.BudgetId });
        }
    }
}
