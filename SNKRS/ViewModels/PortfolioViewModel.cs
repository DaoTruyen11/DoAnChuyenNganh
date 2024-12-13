using SNKRS.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SNKRS.ViewModels
{
    public class PortfolioViewModel
    {
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }


		[AllowHtml]
		public string Description { get; set; }

		[Required]
		public string Image { get; set; }

        [Display(Name = "Product Visibility")]
		public bool isVisible { get; set; }

		public IEnumerable<Category> Categories { get; set; }
		public IEnumerable<int> ProductCategories { get; set; }
		public IEnumerable<string> ProductCategoryNames { get; set; }
		public IEnumerable<ProductGallery> ProductGalleries { get; set; }
		public Portfolio portfolio { get; set; }

		public PortfolioViewModel()
		{
			portfolio = new Portfolio(); // Initialize the portfolio property
		}
	}
}