using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SNKRS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SNKRS.Areas.Admin.Controllers
{
    public class RoleController : AdminController
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        public RoleController()
        {
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        }

        public ActionResult Index()
        {
            var roles = roleManager.Roles.ToList();
            return View(roles);
        }

        public void Delete(string Id)
        {
            var role = roleManager.FindById(Id);
            if (role != null)
            {
                roleManager.Delete(role);
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IdentityRole role)
        {
            if (!ModelState.IsValid)
            {
                return View(role);
            }
            else
            {
                roleManager.Create(role);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string Id)
        {
            if (String.IsNullOrEmpty(Id)) return HttpNotFound();
            var role = roleManager.FindById(Id);
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IdentityRole role)
        {
            roleManager.Update(role);
            return RedirectToAction("Index");
        }
    }
}