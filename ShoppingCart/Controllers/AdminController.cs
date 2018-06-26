using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using ShoppingCart.Models;

namespace ShoppingCart.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _shoppingCartDb = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // This action handles the form POST and the upload
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(file.InputStream);

                if (xmlDoc.DocumentElement != null)
                {
                    var nodeList = xmlDoc.DocumentElement.SelectNodes("/Response/Result/Product");
                    
                    foreach (XmlNode node in nodeList)
                    {
                        Dictionary<string, string> productDictionary = new Dictionary<string, string>();
                        foreach (XmlElement xmlElement in node.SelectNodes("field"))
                        {
                            if (xmlElement.HasAttributes)
                            {
                                productDictionary.Add(xmlElement.Attributes["name"].Value,
                                    xmlElement.Attributes["value"].Value);
                            }
                        }

                        var product = new Product
                        {
                            Name = productDictionary["Title"],
                            Description = productDictionary["Description"],
                            Catergory = productDictionary["Title_1043"],
                            Price = float.Parse(productDictionary["Price"], CultureInfo.InvariantCulture.NumberFormat),
                            Stock = double.Parse(productDictionary["Inventory"],
                                CultureInfo.InvariantCulture.NumberFormat),
                            IsOrderable = productDictionary["IsOrderable"] == "1"
                        };
                        _shoppingCartDb.Products.Add(product);
                        _shoppingCartDb.SaveChanges();
                    }
                }
            }
            // redirect back to the index action to show the form once again
            return RedirectToAction("Index");
        }
    }
}