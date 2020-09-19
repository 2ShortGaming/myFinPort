using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using myFinPort.Enums;
using myFinPort.Extensions;
using myFinPort.Models;
using myFinPort.ViewModels;

namespace myFinPort.Controllers
{
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.Account).Include(t => t.BudgetItem).Include(t => t.Owner);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "AccountName");
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName");
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "Email");
            return View();
        }



        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,BudgetItemId,OwnerId,TransactionType,Amount,Memo")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();

                var thisTransaction = db.Transactions.Include(t => t.BudgetItem).FirstOrDefault(t => t.Id == transaction.Id);
                thisTransaction.UpdateBalances();

                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "OwnerId", transaction.AccountId);
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName", transaction.BudgetItemId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "Email", transaction.OwnerId);
            return View(transaction);
        }


        // GET: Transactions/CreateDeposit
        public ActionResult CreateDeposit()
        {
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "AccountName");
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName");

            var model = new TransactionsVM();
            model.TransactionType = TransactionType.Deposit;
            return View(model);
        }

        // POST: Transactions/CreateDeposit
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDeposit(TransactionsVM model)
        {
            if (ModelState.IsValid)
            {
                var transaction = new Transaction()
                {
                    AccountId = model.BankAccountId,
                    BudgetItemId = model.BudgetItemId,
                    OwnerId = User.Identity.GetUserId(),
                    TransactionType = model.TransactionType,
                    Created = DateTime.Now,
                    Amount = model.Amount,
                    Memo = model.Memo,
                    IsDeleted = false
                };

                db.Transactions.Add(transaction);
                db.SaveChanges();

                var thisTransaction = db.Transactions.Include(t => t.BudgetItem).FirstOrDefault(t => t.Id == transaction.Id);
                thisTransaction.UpdateBalances();

                return RedirectToAction("Dashboard", "Home");
            }

            ViewBag.ErrorMessage = "Something went wrong.  Please try again.";
            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "AccountName");
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName");

            var tryAgain = new TransactionsVM();
            tryAgain.TransactionType = TransactionType.Deposit;

            return View(tryAgain);
        }


        // GET: Transactions/CreateWithdrawal
        public ActionResult CreateWithdrawal()
        {
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "AccountName");
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName");

            var model = new TransactionsVM();
            model.TransactionType = TransactionType.Withdrawal;
            return View(model);
        }

        // POST: Transactions/CreateWithdrawal
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWithdrawal(TransactionsVM model)
        {
            if (ModelState.IsValid)
            {
                var transaction = new Transaction()
                {
                    AccountId = model.BankAccountId,
                    BudgetItemId = model.BudgetItemId,
                    OwnerId = User.Identity.GetUserId(),
                    TransactionType = model.TransactionType,
                    Created = DateTime.Now,
                    Amount = model.Amount,
                    Memo = model.Memo,
                    IsDeleted = false
                };

                db.Transactions.Add(transaction);
                db.SaveChanges();

                var thisTransaction = db.Transactions.Include(t => t.BudgetItem).FirstOrDefault(t => t.Id == transaction.Id);
                thisTransaction.UpdateBalances();

                return RedirectToAction("Dashboard", "Home");
            }

            ViewBag.ErrorMessage = "Something went wrong.  Please try again.";
            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "AccountName");
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName");

            var tryAgain = new TransactionsVM();
            tryAgain.TransactionType = TransactionType.Withdrawal;

            return View(tryAgain);
        }

        // GET: Transactions/CreateTransfer
        public ActionResult CreateTransfer()
        {
            ViewBag.FromBankAccountId = new SelectList(db.BankAccounts, "Id", "AccountName");
            ViewBag.ToBankAccountId = new SelectList(db.BankAccounts, "Id", "AccountName");

            var model = new TransferVM();
            model.TransactionType = TransactionType.Transfer;
            return View(model);
        }

        // POST: Transactions/CreateWithdrawal
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTransfer(TransferVM model)
        {
            if (ModelState.IsValid)
            {
                var wdTransaction = new Transaction()
                {
                    AccountId = model.FromBankAccountId,
                    BudgetItemId = null,
                    OwnerId = User.Identity.GetUserId(),
                    TransactionType = model.TransactionType,
                    Created = DateTime.Now,
                    Amount = model.Amount,
                    Memo = model.Memo,
                    IsDeleted = false
                };

                var depTransaction = new Transaction()
                {
                    AccountId = model.ToBankAccountId,
                    BudgetItemId = null,
                    OwnerId = User.Identity.GetUserId(),
                    TransactionType = model.TransactionType,
                    Created = DateTime.Now,
                    Amount = model.Amount,
                    Memo = model.Memo,
                    IsDeleted = false
                };

                db.Transactions.Add(wdTransaction);
                db.Transactions.Add(depTransaction);
                db.SaveChanges();

                var thisWdTransaction = db.Transactions.Include(t => t.BudgetItem).FirstOrDefault(t => t.Id == wdTransaction.Id);
                var thisDepTransaction = db.Transactions.Include(t => t.BudgetItem).FirstOrDefault(t => t.Id == depTransaction.Id);
                thisWdTransaction.TransferFunds(thisDepTransaction);

                return RedirectToAction("Dashboard", "Home");
            }

            ViewBag.ErrorMessage = "Something went wrong.  Please try again.";
            ViewBag.FromBankAccountId = new SelectList(db.BankAccounts, "Id", "AccountName");
            ViewBag.ToBankAccountId = new SelectList(db.BankAccounts, "Id", "AccountName");

            var tryAgain = new TransferVM();
            tryAgain.TransactionType = TransactionType.Withdrawal;

            return View(tryAgain);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "OwnerId", transaction.AccountId);
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName", transaction.BudgetItemId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "Email", transaction.OwnerId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,BudgetItemId,OwnerId,TransactionType,Created,Amount,Memo,IsDeleted")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var oldTransaction = db.Transactions.AsNoTracking(); // this is a hint, not complete line of code
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();

                //var newTransaction = db.Transactions.AsNoTracking(); // this is a hint, not complete line of code
                /*newTransaction.EditTransaction(oldTransaction);*/

                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "AccountName", transaction.AccountId);
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName", transaction.BudgetItemId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "Email", transaction.OwnerId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            transaction.VoidTransaction();
            db.SaveChanges();
            return RedirectToAction("Dashboard", "Home");
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
