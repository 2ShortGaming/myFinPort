using myFinPort.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myFinPort.ViewModels
{
    public class BuildHouseWizardVM
    {
        public bool IsPersonalAccount { get; set; }
        public decimal StartingBalance { get; set; }
        public Household Household { get; set; }
        public BankAccount BankAccount { get; set; }
        public Budget Budget { get; set; }
        public BudgetItem BudgetItem { get; set; }        

    }
}