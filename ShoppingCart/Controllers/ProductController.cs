using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingCart.Models;

namespace ShoppingCart.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _shoppingCartDb = new ApplicationDbContext();

        // GET: Product
        public ActionResult Index()
        {
            return View(_shoppingCartDb.Products.ToList());
        }

        // GET: Product/Details/5
        public ActionResult Details(int id)
        {
            var product = _shoppingCartDb.Products
                .Single(p => p.ProductId == id);
            if (product != null)
            {
                return View(product);
            }

            return HttpNotFound("Product with id " + id + " not found");
        }
    }
}
