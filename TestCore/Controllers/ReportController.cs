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

        private void SetDropDownLists(long? productId = null, long? locationId = null,
                                        bool isAllNeed = true)
        {

            List<Location> locs = _locRep.GetLocations();
            if (isAllNeed)
            {
                locs.Insert(0,
                    new Location()
                    {
                        LocationId = -1,
                        LocationName = "All"
                    });
            }

            List<Product> prods = _prodRep.GetProducts() as List<Product>;
            if (isAllNeed)
            {
                prods.Insert(0, new Product()
                {
                    ProductId = -1,
                    ProductName = "All"
                });
            }

            if (locationId == null)
                ViewBag.VBLocationlist = new SelectList(locs, "LocationId", "LocationName");
            else
                ViewBag.VBLocationlist = new SelectList(locs, "LocationId", "LocationName", locationId);

            if (productId == null)
                ViewBag.VBProductList = new SelectList(prods, "ProductId", "ProductCodeName");
            else
                ViewBag.VBProductList = new SelectList(prods, "ProductId", "ProductCodeName", productId);
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

        public IActionResult ItemLedger(StockDetailReport filter)
        {

            SetDropDownLists(isAllNeed: false);

            long prevBalance = 0;

            if (filter == null)
                return View(filter);

            if (filter.StartDate != null)
            {
                ViewBag.Log = "in start date<br/>";
                prevBalance = _stockRep.GetBalanceQty(filter.LocationId, 
                                                filter.ProductId, 
                                                filter.StartDate.Value);
                ViewBag.Log += prevBalance;
            }
            filter.Result = _stockRep.Search(
                                    filter.StartDate, filter.EndDate,
                                    filter.LocationId,
                                    filter.ProductId,
                                    StockMovementType.All
                                    );


            ViewBag.PreviousBalance = prevBalance;
            return View(filter);
        }

        public IActionResult ItemLedgerSummary(StockDetailReport filter)
        {

            SetDropDownLists(isAllNeed: false);

            long prevBalance = 0;

            if (filter == null)
                return View(filter);

            if (filter.StartDate != null)
            {
                prevBalance = _stockRep.GetBalanceQty(filter.LocationId,
                                                filter.ProductId,
                                                filter.StartDate.Value);
            }
            filter.Result = _stockRep.GetItemLedgerSummary(
                                    filter.StartDate, filter.EndDate,
                                    filter.LocationId,
                                    filter.ProductId
                                    );


            ViewBag.PreviousBalance = prevBalance;
            return View(filter);
        }
    }
}