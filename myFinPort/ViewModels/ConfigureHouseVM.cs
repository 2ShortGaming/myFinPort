using myFinPort.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myFinPort.ViewModels
{
    public class ConfigureHouseVM
    {
        public int? HouseholdId { get; set; }

        #region Current Options being used
        public decimal StartingBalance { get; set; }
        public BankAccount BankAccount { get; set; }
        public Budget Budget { get; set; }
        public BudgetItem BudgetItem { get; set; }
        #endregion

        #region Option One: Only One instance of each
        //public decimal StartingBalance { get; set; }
        //public decimal WarningBalance { get; set; }
        //public string AccountName { get; set; }
        //public BankAccount BankAccount { get; set; }
        //public Budget Budget { get; set; }
        //public BudgetItem BudgetItem { get; set; }
        #endregion

        #region Option Two: Many collections of each
        //public ICollection<BankAccount> BankAccounts { get; set; }
        //public ICollection<Budget> Budgets { get; set; }
        public ICollection<BudgetItem> BudgetItems { get; set; }
        #endregion

        #region Options three: Hybrid of One and m=Many
        //public BankAccount BankAccount { get; set; }
        //public Budget Budget { get; set; }
        // public ICollection<BudgetItem> BudgetItems { get; set; }

        #endregion

        #region Option Four: Collections of View Models
        //public ICollection<BankAccountWizardVM> BankAccounts { get; set; }
        //public ICollection<BudgetWizardVM> Budgets { get; set; }
        #endregion
    }
}