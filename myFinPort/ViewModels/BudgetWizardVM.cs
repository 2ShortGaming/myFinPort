using myFinPort.Models;
using System.Collections;
using System.Collections.Generic;

namespace myFinPort.ViewModels
{
    public class BudgetWizardVM
    {
        public string BudgetName { get; set; }

        public decimal TargetAmount { get; set; }

        public ICollection<BudgetItem> BudgetItem { get; set; }
    }
}