using myFinPort.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myFinPort.Helpers
{
    public class BudgetsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int BudgetsCount()
        {
            return db.Budgets.ToList().Count;
        }

        public List<Budget> ListBudgets()
        {
            List<Budget> budgets = new List<Budget>();

            budgets.AddRange(db.Budgets);

            return budgets;
        }
    }
}