using Microsoft.AspNet.Identity;
using PortfolioWeb.Models;
using SNKRS.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace PortfolioWeb.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // Action for replying to a comment
        [HttpPost]
        [Authorize]  // Ensure only logged-in users can reply
        public ActionResult Reply(int commentId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return RedirectToAction("PostDetails", "Post", new { id = commentId });  // Redirect to post details if reply is empty
            }

            // Get the original comment to reply to
            var originalComment = db.Comments.Find(commentId);

            if (originalComment == null)
            {
                return HttpNotFound();  // If the comment doesn't exist, return 404
            }

            // Create a new comment (reply)
            var userId = User.Identity.GetUserId();  // Assuming you're using ASP.NET Identity
            var reply = new Comment
            {
                PostId = originalComment.PostId,  // The reply belongs to the same post
                UserId = userId,
                Text = text,
                CreatedAt = DateTime.Now,
                ParentCommentId = commentId  // Set ParentCommentId to the original comment's Id
            };

            db.Comments.Add(reply);  // Add the reply to the context
            db.SaveChanges();  // Save changes to the database

            return RedirectToAction("PostDetails", "Post", new { id = originalComment.PostId });  // Redirect back to the post details page
        }
    

        // Thêm Like cho bài viết
        [HttpPost]
        public JsonResult LikePost(int postId)
        {
            var post = db.Portfolios.Find(postId);
            var userId = User.Identity.GetUserId();  // Lấy UserId hiện tại

            if (post == null)
            {
                return Json(new { status = "error", message = "Bài đăng không tồn tại." });
            }

            // Kiểm tra nếu người dùng đã thích bài đăng này
            var existingLike = db.Likes.FirstOrDefault(l => l.PostId == postId && l.UserId == userId);

            if (existingLike != null)  // Nếu đã thích, xóa like
            {
                db.Likes.Remove(existingLike);
                db.SaveChanges();
                var likeCount = db.Likes.Count(l => l.PostId == postId);
                return Json(new { status = "success", likeCount = likeCount, action = "unlike" });  // Trả về thông báo "unlike"
            }
            else  // Nếu chưa thích, thêm like mới
            {
                var like = new Like { PostId = postId, UserId = userId };
                db.Likes.Add(like);
                db.SaveChanges();

                var likeCount = db.Likes.Count(l => l.PostId == postId);
                return Json(new { status = "success", likeCount = likeCount, action = "like" });  // Trả về thông báo "like"
            }
        }
        // Trong Controller của bạn
        public ActionResult Index()
        {
            // Truy vấn các bài viết và bao gồm số lượng bình luận và các thông tin khác
            var posts = db.Portfolios
                          .Select(post => new
                          {
                              post.Id,
                              post.Name,
                              post.Image,
                              post.Description,
                              LikeCount = post.Likes.Count,
                              CommentCount = post.Comments.Count,  // Lấy số lượng bình luận
                              Comments = post.Comments.OrderBy(c => c.CreatedAt)  // Lấy tất cả bình luận, sắp xếp theo ngày tạo
                          }).ToList();

            // Trả về danh sách bài viết cùng số lượng bình luận
            return View(posts);
        }

        [HttpGet]
        public JsonResult GetCommentsCount(int postId)
        {
            var commentCount = db.Comments.Count(c => c.PostId == postId);
            return Json(new { status = "success", commentCount = commentCount }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult CommentPost(int postId, string commentText)
        {
            var user = User.Identity.IsAuthenticated ? db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name) : null;
            if (user == null || string.IsNullOrEmpty(commentText))
            {
                return Json(new { status = "error", message = "Bình luận không hợp lệ." });
            }

            // Tạo bình luận mới
            var comment = new Comment
            {
                PostId = postId,
                UserId = user.Id,
                Text = commentText,
                CreatedAt = DateTime.Now
            };

            // Lưu bình luận vào cơ sở dữ liệu
            db.Comments.Add(comment);
            db.SaveChanges();

            // Lấy số lượng bình luận sau khi thêm mới
            var commentCount = db.Comments.Count(c => c.PostId == postId);

            return Json(new { status = "success", commentCount });  // Trả về số bình luận mới
        }






        // Lấy bình luận của bài viết
        [HttpGet]
        public JsonResult GetComments(int postId)
        {
            var comments = db.Comments
                              .Where(c => c.PostId == postId)
                              .Include(c => c.User)
                              .OrderByDescending(c => c.CreatedAt)
                              .Select(c => new
                              {
                                  c.User.UserName,
                                  c.Text,
                                  c.CreatedAt,
                                  c.User.ProfileImage
                              })
                              .ToList();

            // Chuyển đổi CreatedAt thành chuỗi và thêm "Ngày" hoặc "Đêm"
            var formattedComments = comments.Select(c => new
            {
                c.UserName,
                c.Text,
                CreatedAt = GetFormattedDateTime(c.CreatedAt),
                c.ProfileImage
            }).ToList();

            return Json(formattedComments, JsonRequestBehavior.AllowGet);
        }

        private string GetFormattedDateTime(DateTime createdAt)
        {
            // Kiểm tra nếu CreatedAt là UTC và chuyển đổi sang giờ địa phương
            DateTime localTime = createdAt.Kind == DateTimeKind.Utc ? createdAt.ToLocalTime() : createdAt;

            // Định dạng lại thời gian
            string formattedDate = localTime.ToString("dd/MM/yyyy HH:mm");

            // Kiểm tra xem giờ có thuộc buổi sáng, chiều hay đêm
            string dayOrNight = localTime.Hour >= 6 && localTime.Hour < 18 ? "PM" : "AM";

            return $"{formattedDate} {dayOrNight}";
        }





    }
}
