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

        public int GetTransactions()
        {
            return db.Transactions.ToList().Count;
        }
    }
}