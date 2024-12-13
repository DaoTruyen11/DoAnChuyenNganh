using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SNKRS.Areas.Admin.ViewModels;
using SNKRS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SNKRS.Areas.Admin.Controllers
{
    public class UserController : AdminController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        public UserController()
        {
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        }
        public ActionResult Index()
        {
            var users = userManager.Users.ToList();
            var viewModels = new List<UserViewModel>();
            foreach (var user in users)
            {
                var viewModel = new UserViewModel()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Roles = userManager.GetRoles(user.Id),
                };
                viewModels.Add(viewModel);
            }
            return View(viewModels);
        }
       

        public ActionResult Edit(string Id)
        {
            var user = userManager.FindById(Id);
            var roles = userManager.GetRoles(user.Id).ToList();
            var viewModel = new UserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Roles = roleManager.Roles.Where(r => roles.Contains(r.Name)).Select(r => r.Id).ToList(),
                AllRole = roleManager.Roles.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel viewModel)
        {
            var roles = userManager.GetRoles(viewModel.Id);
            userManager.RemoveFromRoles(viewModel.Id, roles.ToArray());
            if (viewModel.Roles != null)
            {
                foreach (var item in viewModel.Roles)
                {
                    userManager.AddToRole(viewModel.Id, roleManager.FindById(item).Name);
                }
            }
            var user = userManager.FindById(User.Identity.GetUserId());
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);
            var user = new ApplicationUser()
            {
                Email = viewModel.Email,
                UserName = viewModel.Email,
                Name = viewModel.Name,
            };
            userManager.Create(user, viewModel.Password);
            db.SaveChanges();
            return RedirectToAction("Edit", new { Id = user.Id });
        }
    }
}