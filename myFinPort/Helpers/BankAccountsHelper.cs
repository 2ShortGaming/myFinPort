using myFinPort.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myFinPort.Helpers
{
    public class BankAccountsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int BankAccountsCount()
        {
            return db.BankAccounts.ToList().Count;
        }

        public List<BankAccount> ListBankAccounts()
        {
            List<BankAccount> accounts = new List<BankAccount>();

            accounts.AddRange(db.BankAccounts);

            return accounts;
        }
    }
}