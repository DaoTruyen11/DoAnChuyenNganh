using Microsoft.AspNet.Identity.EntityFramework;
using PortfolioWeb.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace SNKRS.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("ApplicationDbContext", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create() => new ApplicationDbContext();

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ProductGallery> ProductGalleries { get; set; }
        public virtual DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public virtual DbSet<Like> Likes { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Friendship> Friendships { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Portfolio>()
                .HasMany(p => p.Categories)
                .WithMany(c => c.Products)
                .Map(pc =>
                {
                    pc.MapLeftKey("ProductId");
                    pc.MapRightKey("CategoryId");
                    pc.ToTable("ProductCategory");
                });

            modelBuilder.Entity<Portfolio>()
                .HasMany(e => e.ProductGalleries)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.ProductId);

            base.OnModelCreating(modelBuilder);
        }

        public System.Data.Entity.DbSet<SNKRS.ViewModels.PortfolioViewModel> PortfolioViewModels { get; set; }
        
    }
}
