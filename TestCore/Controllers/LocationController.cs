using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using TestCore.Models;
using TestCore.Models.IRepository;
using TestCore.Models.FakeRepository;
using TestCore.Models.SqlRepository;
using TestCore.Helper;

namespace TestCore.Controllers
{
    
    public class LocationController : Controller
    {
        ILocationRepository rep = new SqlLocationRepository();

        [AuthRequire]
        public IActionResult Index()
        {
            return View(rep.GetLocations());
        }

        [AuthRequire]
        public IActionResult Create()
        {
            return View();
        }

        [AuthRequire]
        public IActionResult Edit(long id)
        {
            var found = rep.Find(id);

            if(found != null)
            {
                ViewData.Model = found;
                return View();
            }

            return View("Index", rep.GetLocations());
        }

        [HttpPost]
        public IActionResult Save(Location l)
        {
            if(ModelState.IsValid)
            {
                rep.Add(l);

                return View("Index", rep.GetLocations());
            }

            return View("Create");
        }

        [HttpPost]
        public IActionResult Update(Location l)
        {
            if(ModelState.IsValid)
            {
                rep.Edit(l);

                return View("Index", rep.GetLocations());
            }

            return View("Edit");

        }

        [AuthRequire]
        public IActionResult Delete(long id)
        {
            rep.Remove(id);

            return View("Index", rep.GetLocations());
        }
    }
}