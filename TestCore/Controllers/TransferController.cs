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
    public class TransferController : Controller
    {
        ITransferRepository rep = new SqlTransferRepository();
        IProductRepository _prodRep = new SqlProductRepository();
        ILocationRepository _locRep = new SqlLocationRepository();

        [AuthRequire]
        public IActionResult Index()
        {
            return View(rep.GetTransfers());
        }

        private void SetDropDownLists()
        {
            ViewBag.VBLocationList = _locRep.GetLocations();
            ViewBag.VBProductList = _prodRep.GetProducts();
        }

        [AuthRequire]
        public IActionResult Create()
        {
            SetDropDownLists();
            return View();
        }

        [HttpPost]
        public IActionResult Save(SaleMovement movement)
        {
            if(ModelState.IsValid)
            {
                rep.Add(movement);
                return View("Index", rep.GetTransfers());
            }

            return View("Create");
        }

        [AuthRequire]
        public IActionResult Edit(long id)
        {
            var found = rep.Find(id);

            if(found != null)
            {
                SetDropDownLists();
                return View(found);
            }

            return View("Index", rep.GetTransfers());
        }

        [HttpPost]
        public IActionResult Update(SaleMovement movement)
        {
            if (ModelState.IsValid)
            {
                var found = rep.Find(movement.StockMovementId.Value);
                if (found != null)
                {
                    rep.Edit(movement);
                    return View("Index", rep.GetTransfers());
                }
            }

            return View("Create");
        }

        [AuthRequire]
        public IActionResult Remove(long id)
        {
            rep.Remove(id);

            return View("Index", rep.GetTransfers());
        }
    }
}