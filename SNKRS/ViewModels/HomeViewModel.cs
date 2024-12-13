using SNKRS.Models;
using System.Collections.Generic;

namespace SNKRS.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Portfolio> Upcoming { get; set; }
        public IEnumerable<Portfolio> Best_selling { get; set; }
        public IEnumerable<Portfolio> OnSale { get; set; }
        public IEnumerable<Portfolio> Portfolios { get; set; }

    }
}