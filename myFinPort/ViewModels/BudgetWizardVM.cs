using myFinPort.Models;
using System.Collections;
using System.Collections.Generic;

namespace myFinPort.ViewModels
{
    public class BudgetWizardVM
    {
        public BudgetWizardVM Budget { get; set; }

        public ICollection<BudgetItem> BudgetItem { get; set; }
    }
}