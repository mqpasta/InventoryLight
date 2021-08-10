﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

using TestCore.Models;
using TestCore.Models.FakeRepository;
using TestCore.Models.IRepository;
using TestCore.Models.SqlRepository;
using TestCore.Helper;

namespace TestCore.Controllers
{
    [AuthRequire]
    public class ReceiveController : Controller
    {
        ILocationRepository _locRep = new SqlLocationRepository();
        IPurchaseOrderRepository _ordRep = new SqlPurchaseOrderRepository();
        IPurchaseRepository rep = new SqlPurchaseRepository();

        public IActionResult Index(long id)
        {
            var found = _ordRep.Find(id);

            if (found == null)
            {
                ModelState.AddModelError("Error", "Unable to find Purchase Order");
                return RedirectToAction("Index", "PurchaseOrder");
            }

            ViewBag.PurchaseOrder = found;
            ViewBag.PurchaseMovements = (List<PurchaseMovement>)rep.GetPurchases(id);


            PurchaseMovement movement = new PurchaseMovement();
            movement.ProductId = found.ProductId;
            movement.PurchaseOrderId = found.PurchaseOrderId;
            movement.PurchasePrice = found.PKRCost;
            movement.Date = DateTime.Today;

            SetDropDownLists();
            return View(movement);
        }

        public IActionResult Save(PurchaseMovement movement)
        {
            movement.MovementType = StockMovementType.Purchase;
            rep.Add(movement);

            return RedirectToAction("Index", "Receive", new { id = movement.PurchaseOrderId.Value });
        }

        private void SetDropDownLists()
        {
            ViewBag.VBLocationList = _locRep.GetLocations();
        }

        [HttpGet]
        public IActionResult Edit(long id)
        {

            PurchaseMovement m = rep.Find(id);

            SetDropDownLists();
            return PartialView("_EditPurchasePartial", m);
        }

        [HttpPost]
        public IActionResult Edit(PurchaseMovement purchase)
        {
            var found = rep.Find(purchase.StockMovementId.Value);
            if(found != null)
            {
                found.ToLocationId = purchase.ToLocationId;
                found.Quantity = purchase.Quantity;
                found.Date = purchase.Date;
                rep.Edit(found);
            }

            return Content("done");
        }

        public IActionResult Delete(long id)
        {
            var found = rep.Find(id);
            long poId = 0;

            if (found != null)
            {
                poId = found.PurchaseOrderId.Value;
                rep.Remove(id);
            }
            else
            {
                ModelState.AddModelError("Error", "Unable to delete the record.");
            }

            return RedirectToAction("Index", "Receive", new { id = poId });
        }
    }
}