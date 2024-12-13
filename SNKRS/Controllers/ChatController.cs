using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PortfolioWeb.Models;
using PortfolioWeb.Services;
using SNKRS.Models;

namespace SNKRS.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        private readonly GeminiService _geminiService = new GeminiService();

        // Phương thức gửi tin nhắn đến Gemini và nhận phản hồi
        public async Task<ActionResult> SendToGemini(string userMessage)
        {
            if (string.IsNullOrEmpty(userMessage))
            {
                return Json(new { status = "error", message = "Message content is empty." });
            }

            try
            {
                var geminiResponse = await _geminiService.GetChatResponse(userMessage);
                return Json(new { status = "success", response = geminiResponse });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message });
            }
        }
    

        public ActionResult Index(string recipientUserName)
        {
            var currentUser = User.Identity.Name;
            var messages = _db.ChatMessages
                             .Where(m => (m.Sender.UserName == currentUser && m.Recipient.UserName == recipientUserName) ||
                                         (m.Sender.UserName == recipientUserName && m.Recipient.UserName == currentUser))
                             .OrderBy(m => m.SentAt)
                             .ToList();

            ViewBag.Users = _db.Users.ToList();
            return View(messages);
        }

        public ActionResult Index1(string recipientUserName)
        {
           
            return View();
        }
        public async Task<ActionResult> SendMessage(string recipientUserName, string content)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Recipient: {recipientUserName}, Content: {content}");

                var senderId = User.Identity.GetUserId();
                var recipient = _db.Users.FirstOrDefault(u => u.UserName == recipientUserName);

                if (recipient == null || string.IsNullOrEmpty(content))
                {
                    return Json(new { status = "error", message = "Recipient or content is invalid." });
                }

                var message = new ChatMessage
                {
                    SenderId = senderId,
                    RecipientId = recipient.Id,
                    Content = content,
                    SentAt = DateTime.Now
                };

                _db.ChatMessages.Add(message);
                await _db.SaveChangesAsync();

                return Json(new
                {
                    status = "success",
                    sentAt = message.SentAt.ToString("g"),
                    content = message.Content
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message });
            }
        }

        public ActionResult GetMessages(string recipientUserName)
        {
            var currentUser = User.Identity.Name;
            var messages = _db.ChatMessages
                 .Where(m => (m.Sender.UserName == currentUser && m.Recipient.UserName == recipientUserName) ||
                             (m.Sender.UserName == recipientUserName && m.Recipient.UserName == currentUser))
                 .OrderBy(m => m.SentAt)
                 .ToList() // Thực hiện truy vấn và tải dữ liệu về bộ nhớ
                 .Select(m => new
                 {
                     isSent = m.Sender.UserName == currentUser,
                     content = m.Content,
                     sentAt = m.SentAt.ToString("g")  // Chuyển đổi sau khi dữ liệu đã ở bộ nhớ
                 })
                 .ToList();

            return Json(messages, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Chat()
        {
            var currentUserId = User.Identity.GetUserId();  // Lấy ID người dùng hiện tại

            // Lấy danh sách bạn bè đã chấp nhận của người dùng hiện tại
            var acceptedFriends = _db.Friendships
                .Where(f => (f.UserId == currentUserId || f.FriendId == currentUserId) && f.Status == "Accepted")
                .Select(f => f.UserId == currentUserId ? f.FriendId : f.UserId)
                .ToList();

            // Lấy thông tin người dùng từ danh sách bạn bè đã chấp nhận
            var users = _db.Users
                .Where(u => acceptedFriends.Contains(u.Id))
                .ToList();

            ViewBag.Users = users;  // Truyền danh sách bạn bè đã chấp nhận vào View
            return View();
        }




    }
}
