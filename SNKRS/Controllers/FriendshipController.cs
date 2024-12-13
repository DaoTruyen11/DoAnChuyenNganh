using Microsoft.AspNet.Identity;
using SNKRS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortfolioWeb.Controllers
{
    public class FriendshipController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Gửi yêu cầu kết bạn
        [HttpPost]
        public ActionResult SendRequest(string friendId)
        {
            var userId = User.Identity.GetUserId();

            if (friendId == userId)
            {
                TempData["ErrorMessage"] = "You cannot send a friend request to yourself.";
                return RedirectToAction("AvailableFriends");
            }

            var existingRequest = db.Friendships
                                    .FirstOrDefault(f => (f.UserId == userId && f.FriendId == friendId) ||
                                                         (f.UserId == friendId && f.FriendId == userId));

            if (existingRequest == null)
            {
                var friendship = new Friendship
                {
                    UserId = userId,
                    FriendId = friendId,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                db.Friendships.Add(friendship);
                db.SaveChanges();
            }
            else
            {
                TempData["ErrorMessage"] = "You have already sent a friend request.";
            }

            return RedirectToAction("AvailableFriends");
        }

        // Chấp nhận yêu cầu kết bạn
        [HttpPost]
        public ActionResult AcceptRequest(int id)
        {
            var friendship = db.Friendships.Find(id);

            if (friendship == null)
            {
                TempData["ErrorMessage"] = "Friend request not found.";
                return RedirectToAction("Index", "Friendship");
            }

            if (friendship.FriendId == User.Identity.GetUserId())
            {
                friendship.Status = "Accepted";
                db.SaveChanges();
            }

            return RedirectToAction("FriendList", "Friendship");
        }

        // Từ chối yêu cầu kết bạn
        [HttpPost]
        public ActionResult RejectRequest(int id)
        {
            var friendship = db.Friendships.Find(id);

            if (friendship == null)
            {
                TempData["ErrorMessage"] = "Friend request not found.";
                return RedirectToAction("Index", "Friendship");
            }

            if (friendship.FriendId == User.Identity.GetUserId())
            {
                friendship.Status = "Rejected";
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Friendship");
        }

        // Danh sách yêu cầu kết bạn
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            // Lấy danh sách yêu cầu kết bạn mà người dùng đã nhận (trạng thái "Pending")
            var friendshipRequests = db.Friendships
                .Where(f => f.FriendId == userId && f.Status == "Pending")
                .ToList();

            ViewBag.FriendshipRequests = friendshipRequests;

            return View();
        }

        // Danh sách bạn bè đã chấp nhận
        public ActionResult FriendList()
        {
            var userId = User.Identity.GetUserId();

            var friends = db.Friendships
                            .Where(f => (f.UserId == userId || f.FriendId == userId) && f.Status == "Accepted")
                            .ToList();

            var friendIds = friends
                            .Select(f => f.UserId == userId ? f.FriendId : f.UserId)
                            .Distinct()
                            .ToList();

            var friendList = db.Users
                                .Where(u => friendIds.Contains(u.Id))
                                .ToList();

            return View(friendList);
        }

        // Danh sách người dùng có thể kết bạn
        public ActionResult AvailableFriends()
        {
            var userId = User.Identity.GetUserId();

            // Lấy danh sách userId đã gửi hoặc nhận yêu cầu kết bạn
            var friendIds = db.Friendships
                              .Where(f => f.UserId == userId || f.FriendId == userId)
                              .Select(f => f.UserId == userId ? f.FriendId : f.UserId)
                              .ToList();

            // Lấy danh sách người dùng chưa kết bạn
            var availableFriends = db.Users
                                     .Where(u => u.Id != userId && !friendIds.Contains(u.Id))
                                     .ToList();

            return View(availableFriends);
        }
    }
}