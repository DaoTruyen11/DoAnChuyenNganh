using SNKRS.Models;

namespace PortfolioWeb.Models
{
    public class Like
    {
        public int Id { get; set; }

        public int PostId { get; set; }  // Chỉ định bài đăng mà người dùng thích
        public virtual Portfolio Post { get; set; }  // Mối quan hệ với bài đăng

        public string UserId { get; set; }  // Mã người dùng
        public virtual ApplicationUser User { get; set; }  // Mối quan hệ với người dùng
    }
}
