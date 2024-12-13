using SNKRS.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

public class Friendship
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public string FriendId { get; set; }

    [Required]
    public string Status { get; set; } // "Pending", "Accepted", "Rejected"

    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }

    [ForeignKey("FriendId")]
    public virtual ApplicationUser Friend { get; set; }
}
