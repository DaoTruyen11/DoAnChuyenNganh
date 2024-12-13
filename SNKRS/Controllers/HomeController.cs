using SNKRS.Models;
using SNKRS.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace SNKRS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;

        public HomeController()
        {
            db = new ApplicationDbContext();
        }
        public ActionResult Index()
        {
            var model = new HomeViewModel
            {
                Portfolios = db.Portfolios.Where(p => p.isVisible).OrderBy(p => p.Id).OrderByDescending(p => p.Id).Take(20).ToList()  // Lọc các portfolio hiển thị
            };

            return View(model);
        }



       
    }
}