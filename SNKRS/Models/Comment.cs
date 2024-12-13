using SNKRS.Models;
using System;
using System.Collections.Generic;

public class Comment
{
    public int Id { get; set; }
    public int PostId { get; set; }  // Phải có giá trị hợp lệ
    public string UserId { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }

    // Thêm thuộc tính ParentCommentId để xác định bình luận gốc (nếu có)
    public int? ParentCommentId { get; set; }  // Có thể null nếu không phải bình luận trả lời

    // Thêm thuộc tính để lưu trữ bình luận trả lời
    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();  // Các bình luận trả lời

    // Liên kết đến bình luận cha (nếu có)
    public virtual Comment ParentComment { get; set; }

    public virtual ApplicationUser User { get; set; }
}
