﻿using myFinPort.Enums;
using myFinPort.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myFinPort.Extensions
{
    public static class TransactionExtensions
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        // what does a transaction do?
        // if transactiontype.deposit - increases the current amount in the BankAccunt it was deposited to
        // if transactiontype.Withdrawal - reduces the current amount of th BankAccount, increases the current amount of Budget and BudgetItem
        // Optional: TransactionType.Transfer - reduces current amount of account1 and increases current amount of account2

        // this code is called in the Account Controller - updates need to be made in both locations together.

        // chaining method calls under a public method.  The tree methods calls in the method body will be private static void methods
        public static void UpdateBalances(this Transaction transaction)
        {
            UpdateBankBalance(transaction);

            // deposits do not affect Budget or BudgetItem so we can test for the transaction type before calling those methods
            if (transaction.TransactionType == TransactionType.Withdrawal)
            {
                UpdateBudgetAmount(transaction);
                UpdateBudgetItemAmount(transaction);
            }
        }

        public static void EditTransaction(this Transaction newTransaction, Transaction oldTransaction)
        {
            return;
        }

        private static void UpdateBankBalance(Transaction transaction)
        {
            var bankAccount = db.BankAccounts.Find(transaction.AccountId);

            if(transaction.TransactionType == TransactionType.Deposit)
            {
                
                bankAccount.CurrentBalance += transaction.Amount;
            }
            else if(transaction.TransactionType == TransactionType.Withdrawal)
            {
                bankAccount.CurrentBalance -= transaction.Amount;
            }

            db.SaveChanges();
        }

        private static void UpdateBudgetAmount(Transaction transaction)
        {
            var budgetItem = db.BudgetItems.Find(transaction.BudgetItemId);
            var budgetId = budgetItem.BudgetId;
            var budget = db.Budgets.Find(budgetId);
            budget.CurrentAmount += transaction.Amount;
            db.SaveChanges();
        }

        private static void UpdateBudgetItemAmount(Transaction transaction)
        {
            var budgetItem = db.BudgetItems.Find(transaction.BudgetItemId);
            budgetItem.CurrentAmount += transaction.Amount;
            db.SaveChanges();
        }

        // additional functionality you need to write
        // what happens when I edit a transaction? - hint you might need a momento object .AsNoTracking()
        // momento object means grab the transaction before it was edited so you'll have that to compare to the edited version
        // what happens when I delete or void a transaction? - part of these methods involve tracking the old amount

    }
}