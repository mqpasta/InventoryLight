using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TestCore.Models;
using TestCore.Models.FakeRepository;
using TestCore.Models.IRepository;
using TestCore.Models.SqlRepository;
using TestCore.Helper;

namespace TestCore.Controllers
{
    public class PurchaseController : Controller
    {
        IPurchaseRepository rep = new SqlPurchaseRepository();
        ILocationRepository _locRep = new SqlLocationRepository();
        IProductRepository _prodRep = new SqlProductRepository();

        public PurchaseController()
        {
            
        }

        [AuthRequire]
        public IActionResult Index()
        {
            return View(rep.GetPurchases());
        }

        [AuthRequire]
        public IActionResult Create()
        {
            SetDropDownLists();
            return View();
        }

        private void SetDropDownLists()
        {
            ViewBag.VBLocationList = _locRep.GetLocations();
            ViewBag.VBProductList = _prodRep.GetProducts();
        }

        [HttpPost]
        public IActionResult Save(PurchaseMovement purchase)
        {
            if(ModelState.IsValid)
            {
                rep.Add(purchase);
                return View("Index", rep.GetPurchases());
            }

            return View("Create", purchase);
        }

        [AuthRequire]
        public IActionResult Edit(long id)
        {
            var found = rep.Find(id);
            SetDropDownLists();

            return View(found);
        }

        [HttpPost]
        public IActionResult Update(PurchaseMovement purchase)
        {
            if (ModelState.IsValid)
            {
                var found = rep.Find(purchase.StockMovementId.Value);
                if(found != null)
                {
                    rep.Edit(purchase);
                }
                return View("Index", rep.GetPurchases());
            }

            return View("Edit");
        }

        [AuthRequire]
        public IActionResult Remove(long id)
        {
            rep.Remove(id);

            return View("Index", rep.GetPurchases());
        }
    }
}