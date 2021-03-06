﻿using myFinPort.Models;
using myFinPort.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myFinPort.Helpers
{
    public class BankAccountsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int HHBankAccountsCount()
        {
            var hhId = HttpContext.Current.User.Identity.GetHouseholdId();

            return db.BankAccounts.Where(hh => hh.HouseholdId == hhId).ToList().Count;
        }

        public List<BankAccount> ListHHBankAccounts()
        {
            List<BankAccount> accounts = new List<BankAccount>();
            var hhId = HttpContext.Current.User.Identity.GetHouseholdId();

            accounts.AddRange(db.BankAccounts.Where(hh => hh.HouseholdId == hhId).ToList());

            return accounts;
        }

        public decimal TotalBalance()
        {
            var accounts = ListHHBankAccounts();
            decimal totalBalance = 0M;

            foreach (var account in accounts)
            {
                totalBalance += account.CurrentBalance;
            }
            return totalBalance;
        }

        public decimal TotalBalance(BankAccount account)
        {
            return account.CurrentBalance;
        }

        // All deposits from ALL accounts in the household
        public decimal TotalDepositsFromDate(DateTime startingDate)
        {
            decimal totalDeposits = 0M;

            // this starts a loop over ALL bank accounts in the household
            foreach (var account in ListHHBankAccounts())
            {
                var transactions = db.Transactions.Where(t => t.AccountId == account.Id).OrderByDescending(b => b.Created);

                // for each account in the household, this runs through their transactions and totals the deposits
                foreach (var transaction in transactions)
                {
                    if (transaction.Created < startingDate)
                    {
                        break;
                    }

                    if (transaction.TransactionType == Enums.TransactionType.Deposit && !transaction.IsDeleted)
                    {
                        totalDeposits += transaction.Amount;
                    }
                }
            }

            // now totalDeposits hold the total deposits for ALL accounts in this household
            return totalDeposits;
        }

        // returns the total deposits for the specified account from the specified date to today.
        public decimal TotalDepositsFromDate(BankAccount account, DateTime startingDate)
        {
            decimal totalDeposits = 0M;
            var transactions = db.Transactions.Where(t => t.AccountId == account.Id).OrderByDescending(b => b.Created);

            foreach (var transaction in transactions)
            {
                if (transaction.Created < startingDate)
                {
                    break;
                }

                if (transaction.TransactionType == Enums.TransactionType.Deposit && !transaction.IsDeleted)
                {
                    totalDeposits += transaction.Amount;
                }
            }

            return totalDeposits;
        }

        public decimal TotalWithdrawalsFromDate(DateTime startingDate)
        {
            decimal totalDeposits = 0M;

            foreach (var account in ListHHBankAccounts())
            {
                var transactions = db.Transactions.Where(t => t.AccountId == account.Id).OrderByDescending(b => b.Created);

                foreach (var transaction in transactions)
                {
                    if (transaction.Created < startingDate)
                    {
                        break;
                    }

                    if (transaction.TransactionType == Enums.TransactionType.Withdrawal && !transaction.IsDeleted)
                    {
                        totalDeposits += transaction.Amount;
                    }
                }
            }
            return totalDeposits;
        }

        public decimal TotalWithdrawalsFromDate(BankAccount account, DateTime startingDate)
        {
            decimal totalDeposits = 0M;
            var transactions = db.Transactions.Where(t => t.AccountId == account.Id).OrderByDescending(b => b.Created);

            foreach (var transaction in transactions)
            {
                if (transaction.Created < startingDate)
                {
                    break;
                }

                if (transaction.TransactionType == Enums.TransactionType.Withdrawal && !transaction.IsDeleted)
                {
                    totalDeposits += transaction.Amount;
                }
            }

            return totalDeposits;
        }

        public decimal GetNetFromDate(DateTime startingDate)
        {
            decimal net = TotalDepositsFromDate(startingDate) - TotalWithdrawalsFromDate(startingDate);
            return net;
        }

        public decimal GetNetFromDate(BankAccount account, DateTime startingDate)
        {
            var deposits = TotalDepositsFromDate(account, startingDate);
            var withdrawals = TotalWithdrawalsFromDate(account, startingDate);

            return deposits - withdrawals;
        }
    }
}