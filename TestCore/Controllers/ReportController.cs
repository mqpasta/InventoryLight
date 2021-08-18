using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using TestCore.Models;
using TestCore.Models.Reports;
using TestCore.Models.SqlRepository;
using TestCore.Models.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TestCore.Controllers
{
    public class ReportController : Controller
    {
        IProductRepository _prodRep = new SqlProductRepository();
        ILocationRepository _locRep = new SqlLocationRepository();
        IPurchaseRepository rep = new SqlPurchaseRepository();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GoodReceive()
        {

            GoodReceiveReport r = new GoodReceiveReport();
            r.PurchaseMovements = new List<PurchaseMovement>();
            r.PurchaseMovements.Add(new PurchaseMovement());

            SetDropDownLists(null);

            return View(r);
        }

        public IActionResult GoodReceiveFilter(GoodReceiveReport filter)
        {
            SetDropDownLists(filter.ProductId);

            return View("GoodReceive", filter);
        }

        private void SetDropDownLists(long? selectedProduct)
        {

            List<Location> locs = _locRep.GetLocations();
            locs.Insert(0,
                new Location()
                {
                    LocationId = -1,
                    LocationName = "All"
                });
            ViewBag.VBLocationList = locs;

            List<Product> prods = _prodRep.GetProducts() as List<Product>;
            prods.Insert(0, new Product()
            {
                ProductId = -1,
                ProductName = "All"
            });


            ViewBag.VBProductList = new SelectList(prods, "ProductId", "ProductCodeName", selectedProduct);
        }
    }
}