using Microsoft.AspNet.Identity;
using SNKRS.Areas.Admin.ViewModels;
using SNKRS.Models;
using SNKRS.ViewModels;
using System.Linq;
using System.Web.Mvc;

[Authorize]
public class PortfolioController : Controller
{
	private readonly ApplicationDbContext db;

	public PortfolioController()
	{
		db = new ApplicationDbContext();
	}

	public ActionResult Index()
	{
		var userId = User.Identity.GetUserId();
		var userPortfolios = db.Portfolios.Where(p => p.UserId == userId).ToList();
		return View(userPortfolios);
	}

	public ActionResult ListPortfolio()
	{
		var portfolios = db.Portfolios.Where(p => p.isVisible).ToList();
		return View(portfolios);
	}

	public ActionResult Details(int? Id)
	{
		if (Id == null) return HttpNotFound();
		var portfolio = db.Portfolios.FirstOrDefault(p => p.Id == Id && p.isVisible);
		if (portfolio == null) return HttpNotFound();
		var model = new PortfolioViewModel();
		model.portfolio = portfolio;
		return View(model);
	}
	public ActionResult Create()
	{
		PortfolioViewModel viewModel = new PortfolioViewModel
		{
			Categories = db.Categories.ToList()
		};
		return View(viewModel);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public ActionResult Create(PortfolioViewModel viewModel)
	{
		if (!ModelState.IsValid)
		{
			viewModel.Categories = db.Categories.ToList();
			return View("Create", viewModel);
		}
		var userId = User.Identity.GetUserId();
		var product = new Portfolio()
		{
			Name = viewModel.Name,
			Image = viewModel.Image,
			Description = viewModel.Description,
			isVisible = viewModel.isVisible,
			UserId = userId
		};
		if (viewModel.ProductCategories != null)
		{
			foreach (var item in viewModel.ProductCategories)
			{
				product.Categories.Add(db.Categories.Single(x => x.Id == item));
			}
		}
		db.Portfolios.Add(product);
		db.SaveChanges();
		return RedirectToAction("Index");
	}

	public ActionResult Edit(int? Id)
	{
		if (Id == null) return HttpNotFound();
		var product = db.Portfolios.SingleOrDefault(p => p.Id == Id);
		if (product == null) return HttpNotFound();
		var viewModel = new PortfolioViewModel
		{
			Id = product.Id,
			Name = product.Name,
			Description = product.Description,
			Image = product.Image,
			ProductCategories = product.Categories.Select(x => x.Id).ToList(),
			Categories = db.Categories.ToList(),
			ProductGalleries = product.ProductGalleries,
			isVisible = product.isVisible,
		};
		return View(viewModel);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public ActionResult Edit(PortfolioViewModel viewModel)
	{
		var product = db.Portfolios.First(p => p.Id == viewModel.Id);
		product.Name = viewModel.Name;
		product.Description = viewModel.Description;
		product.Image = viewModel.Image;
		product.isVisible = viewModel.isVisible;
		product.Categories.Clear();
		if (viewModel.ProductCategories != null)
		{
			foreach (var item in viewModel.ProductCategories)
			{
				product.Categories.Add(db.Categories.Single(x => x.Id == item));
			}
		}
		db.SaveChanges();
		return RedirectToAction("Index");
	}

	public void Delete(int Id)
	{
		var userId = User.Identity.GetUserId();
		var product = db.Portfolios.SingleOrDefault(p => p.Id == Id && p.UserId == userId);
		if (product != null)
		{
			db.Portfolios.Remove(product);
			db.SaveChanges();
		}
	}

	[HttpPost]
	public void AddGallery(int Id, string Src)
	{
		var userId = User.Identity.GetUserId();
		var product = db.Portfolios.FirstOrDefault(x => x.Id == Id && x.UserId == userId);
		if (product != null)
		{
			var productGallery = new ProductGallery()
			{
				ProductId = product.Id,
				Src = Src,
			};
			db.ProductGalleries.Add(productGallery);
			db.SaveChanges();
		}
	}
}
