using myFinPort.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myFinPort.ViewModels
{
    public class TransferVM
    {
        public TransactionType TransactionType { get; set; }
        public int FromBankAccountId { get; set; }
        public int ToBankAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Memo { get; set; }
    }
}