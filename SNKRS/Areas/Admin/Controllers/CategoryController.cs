using SNKRS.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity.Migrations;

namespace SNKRS.Areas.Admin.Controllers
{
    [Authorize]
    public class CategoryController : AdminController
    {
        private readonly ApplicationDbContext db;
        public CategoryController()
        {
            db = new ApplicationDbContext();
        }
        public ActionResult Index()
        {
            var categories = db.Categories.ToList();
            return View(categories);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            db.Categories.Add(category);
            db.SaveChanges();
            return View();
        }

        public void Delete(int Id)
        {
            var category = db.Categories.SingleOrDefault(c => c.Id == Id);
            db.Categories.Remove(category);
            db.SaveChanges();
        }

        public ActionResult Edit(int? Id)
        {
            if (Id == null) return HttpNotFound();
            var category = db.Categories.SingleOrDefault(c => c.Id == Id);
            if (category == null) return HttpNotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            db.Categories.AddOrUpdate(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}