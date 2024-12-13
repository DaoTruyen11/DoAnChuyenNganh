using SNKRS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNKRS.Areas.Admin.ViewModels
{
    public class DashboardViewModel
    {
        public decimal Total { get; set; }
        public int TotalOrders { get; set; }
        public decimal ThisMonth { get; set; }
        public int ThisMonthlOrders { get; set; }
        public decimal Pending { get; set; }
        public int PendingOrders { get; set; }
        public decimal Cancelled { get; set; }
        public int CancelledOrders { get; set; }
        public decimal DayTotal { get; set; }
        public int DayOrder { get; set; }
        public IEnumerable<Portfolio> DayProducts { get; set; }
        public List<int> DayProductsCount { get; set; }
    }
}