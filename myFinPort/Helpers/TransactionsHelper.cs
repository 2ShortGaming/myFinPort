using Microsoft.AspNet.Identity;
using myFinPort.Extensions;
using myFinPort.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myFinPort.Helpers
{
    public class TransactionsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int GetTransactionsCount()
        {
            return db.Transactions.ToList().Count;
        }

        public List<Transaction> GetUserTransactions()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();

            return db.Transactions.Where(t => t.OwnerId == userId).ToList();
        }

        public List<Transaction> GetHHTransactions()
        {
            var hhId = HttpContext.Current.User.Identity.GetHouseholdId();
            var userId = HttpContext.Current.User.Identity.GetUserId();

            var transactions = db.BankAccounts.Where
                (
                    b => b.HouseholdId == hhId && b.OwnerId == userId
                ).SelectMany(b => b.Transactions).ToList();

            return transactions;
        }

        public int GetHHTransactionsCount()
        {
            return GetHHTransactions().Count;
        }

    }
}