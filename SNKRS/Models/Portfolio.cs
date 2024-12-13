using PortfolioWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SNKRS.Models
{
    public class Portfolio
    {
        public Portfolio()
        {
            ProductGalleries = new HashSet<ProductGallery>();
            Categories = new HashSet<Category>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public bool isVisible { get; set; }
        public string UserId { get; set; }
        public string Video { get; set; }  // Thêm trường Video

        public virtual ICollection<ProductGallery> ProductGalleries { get; set; }
        public virtual ICollection<Category> Categories { get; set; }

        // Tính số lượng bình luận và like
        public int LikeCount => this.Likes.Count;
        public int CommentCount => this.Comments.Count;
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
