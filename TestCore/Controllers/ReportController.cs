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
        IPurchaseOrderRepository rep = new SqlPurchaseOrderRepository();
        IStockStatusRepository _stockRep = new SqlStockStatusRepository();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PurchaseOrder()
        {

            PurchaseOrderReport r = new PurchaseOrderReport();
            SetDropDownLists();

            return View(r);
        }

        public IActionResult PurchaseOrderReport(PurchaseOrderReport filter)
        {

            filter.PurchaseOrders = rep.Search(
                filter.StartDate,
                filter.EndDate,
                (filter.LocationId > 0) ? (Nullable<long>)filter.LocationId : null,
                (filter.ProductId > 0) ? (Nullable<long>)filter.ProductId : null,
                (filter.IsReceived) ? (Nullable<bool>)true : null,
                (filter.IsLessQuantity) ? (Nullable<bool>)true : null
                );


            SetDropDownLists(filter.ProductId, filter.LocationId);

            return View("PurchaseOrder", filter);
        }

        private void SetDropDownLists(long? selectedProduct = null, long? locationId = null)
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

            ViewBag.VBLocationlist = new SelectList(ViewBag.VBLocationList, "LocationId", "LocationName", locationId);
            ViewBag.VBProductList = new SelectList(prods, "ProductId", "ProductCodeName", selectedProduct);

        }


        public IActionResult StockDetails()
        {
            var model = new StockDetailReport();
            model.Type = StockMovementType.All;
            SetDropDownLists();
            return View(model);
        }


        public IActionResult GenerateStockDetails(StockDetailReport filter)
        {
            filter.Result = _stockRep.Search(filter.StartDate, filter.EndDate,
                                    (filter.LocationId < 0 ? null as Nullable<long> : filter.LocationId),
                                    (filter.ProductId < 0) ? null as Nullable<long> : filter.ProductId,
                                    (filter.Type == StockMovementType.All) ? null as Nullable<StockMovementType> : filter.Type);

            SetDropDownLists(filter.ProductId, filter.LocationId);
            return View("StockDetails", filter);
        }
    }
}