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
            var postsCount = db.Portfolios.Count();
            var likesCount = db.Likes.Count();
            var commentsCount = db.Comments.Count();

            System.Diagnostics.Debug.WriteLine($"PostsCount: {postsCount}");
            System.Diagnostics.Debug.WriteLine($"LikesCount: {likesCount}");
            System.Diagnostics.Debug.WriteLine($"CommentsCount: {commentsCount}");

            var viewModel = new DashboardViewModel
            {
                PostsCount = postsCount,
                LikesCount = likesCount,
                CommentsCount = commentsCount
            };

            return View(viewModel);
        }

    }
}