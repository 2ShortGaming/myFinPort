using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myFinPort.ViewModels
{
    public class DashboardVM
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DashboardVM()
        {
            StartDate = new DateTime(DateTime.Now.Year, 1, 1);
            EndDate = DateTime.Now;
        }

        public DashboardVM(DateTime start, DateTime end)
        {
            StartDate = start;
            EndDate = end;
        }
    }
}