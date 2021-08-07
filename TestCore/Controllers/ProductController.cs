using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using TestCore.Models;
using TestCore.Models.IRepository;
using TestCore.Models.FakeRepository;
using TestCore.Models.SqlRepository;


namespace TestCore.Controllers
{
    public class ProductController : Controller
    {
        IProductRepository rep = new SqlProductRepository();

        public ProductController(IProductRepository productRepository)
        {
            // Causing reseting the collection
            //rep = new FakeProductRepository();
        }

        public IActionResult Index()
        {
            return View("ShowAll", rep.GetProducts());
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult CreateProduct(Product p)
        {
            if(ModelState.IsValid)
            {
                rep.Add(p);
                return View("ShowAll", rep.GetProducts());
            }

            return View("Create");
            
        }

        public IActionResult List()
        {
            return View("ShowAll", rep.GetProducts());
        }

        public IActionResult ShowAll(List<Product> p)
        {
            return View(p);
        }

        public IActionResult Edit(long id)
        {
            Product p = rep.Find(id);

            ViewData.Model = p;
            return View();
        }

        public IActionResult Update(Product p)
        {
            if(ModelState.IsValid)
            {
                rep.Edit(p);
                return View("ShowAll", rep.GetProducts());
            }

            return View("Edit");
        }

        public IActionResult Remove(long id)
        {
            rep.Remove(id);

            return View("ShowAll", rep.GetProducts());
        }
    }
}