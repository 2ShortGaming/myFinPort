using myFinPort.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myFinPort.ViewModels
{
    public class TransactionsVM
    {
        public TransactionType TransactionType { get; set; }
        public int BankAccountId { get; set; }
        public int BudgetItemId { get; set; }
        public decimal Amount { get; set; }
        public string Memo { get; set; }

    }
}