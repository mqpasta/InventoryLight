using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestCore.Models;
using TestCore.Models.FakeRepository;
using TestCore.Models.IRepository;
using TestCore.Models.SqlRepository;
using TestCore.Helper;


namespace TestCore.Controllers
{
    
    public class StockController : Controller
    {
        IStockStatusRepository rep = new SqlStockStatusRepository();
        IProductRepository _prodRep = new SqlProductRepository();
        ILocationRepository _locRep = new SqlLocationRepository();

        [AuthRequire]
        public IActionResult Index()
        {
            SetDropDownLists();

            return View(rep.GetAllStatus());
        }

        [AuthRequire]
        public IActionResult Wearhouse()
        {
            return View(rep.GetWearhouseStock());
        }

        private void SetDropDownLists()
        {
            ViewBag.VBLocationList = _locRep.GetLocations();
            ViewBag.VBProductList = _prodRep.GetProducts();
        }

        [AuthRequire]
        public IActionResult Search([FromQuery] string LocationId,
                            [FromQuery] string ProductId,
                            [FromQuery] string summarize)
        {
            LocationId = LocationId.ToLower();
            ProductId = ProductId.ToLower();
            bool isSummarize = (summarize == "on") ? true : false;

            List<StockStatus> results;

            if (LocationId == "all" && ProductId == "all")
            {
                results = rep.GetAllStatus(isSummarize);
            }
            else if (ProductId != "all" && LocationId == "all")
            {
                results = rep.GetForProduct(Convert.ToInt64(ProductId), isSummarize);
            }
            else if (ProductId == "all" && LocationId != "all")
            {
                results = rep.GetForLocation(Convert.ToInt64(LocationId), isSummarize);
            }
            else
            {
                results = rep.GetFor(Convert.ToInt64(LocationId),
                                Convert.ToInt64(ProductId), isSummarize);
            }

            SetDropDownLists();
            return View("Index", results);

        }
    }
}