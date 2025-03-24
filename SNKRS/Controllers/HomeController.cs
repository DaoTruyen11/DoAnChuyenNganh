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
                Portfolios = db.Portfolios
                                .Where(p => p.isVisible)  // Lọc các portfolio có trường isVisible là true
                                .OrderByDescending(p => p.Id)  // Sắp xếp giảm dần theo Id
                                .Take(20)  // Lấy 20 bản ghi đầu tiên
                                .ToList()
            };

            return View(model);  // Trả về view với model chứa danh sách các portfolio
        }
    }
}