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
    [AuthRequire]
    public class PurchaseOrderController : Controller
    {
        IPurchaseOrderRepository rep = new SqlPurchaseOrderRepository();
        IProductRepository _prodRep = new SqlProductRepository();

        private void SetDropDownLists()
        {
            ViewBag.VBProductList = _prodRep.GetProducts();
        }

        public IActionResult Index()
        {
            return View(rep.GetOrders());
        }

        public IActionResult Create()
        {
            SetDropDownLists();

            return View();
        }

        public IActionResult Save(PurchaseOrder po)
        {
            if(ModelState.IsValid)
            {
                rep.Add(po);
                return View("Index", rep.GetOrders());
            }

            return View("Create");
        }

        public IActionResult Edit(long id)
        {
            var found = rep.Find(id);

            if(found != null)
            {
                SetDropDownLists();
                return View(found);
            }

            return View("Index", rep.GetOrders());
        }

        public IActionResult Update(PurchaseOrder po)
        {
            if(ModelState.IsValid)
            {
                rep.Edit(po);
                return View("Index", rep.GetOrders());
            }

            return View("Edit");
        }

        public IActionResult Delete(long id)
        {
            var found = rep.Find(id);

            if(found != null)
            {
                try
                {
                    rep.Remove(id);
                    return View("Index", rep.GetOrders());
                }
                catch(Exception e)
                {
                    return ShowError(e.Message);
                }
            }

            return ShowError("Unable to delete the record.");
        }

        private IActionResult ShowError(string error)
        {
            ModelState.AddModelError("Error", error);
            return View("Index", rep.GetOrders());
        }

    }
}