using SNKRS.Models;
using System.Web.Mvc;
using System.Linq;
using System;
using SNKRS.Areas.Admin.ViewModels;

namespace SNKRS.Areas.Admin.Controllers
{
    [Authorize]
    public class DashboardController : AdminController
    {
        private readonly ApplicationDbContext db;
        public DashboardController()
        {
            db = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var viewModel = new DashboardViewModel();
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection form)
        {
            var date = Convert.ToDateTime(form["datePicker"]);
           
           
            var dayProducts = db.Portfolios.SqlQuery($"select P.* from ( select PS.ProductId, Count(PS.ProductId) as Count from Orders O inner join OrderDetails OD on O.Id = OD.OrderId inner join ProductSizes PS on OD.ProductSizeId = PS.Id where Day(O.Created_At) = {date.Day} and Month(O.Created_At) = {date.Month} and Year(O.Created_At) = {date.Year} group by PS.ProductId ) A inner join Products P on P.Id = A.ProductId order by A.Count desc").ToList<Portfolio>();
            var viewModel = new DashboardViewModel()
            {
                
                DayProducts = dayProducts,
            };
            return View(viewModel);
        }
    }
}