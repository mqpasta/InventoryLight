using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestCore.Models;
using TestCore.Helper;
using System.Text;

namespace TestCore.Controllers
{
    public class HomeController : Controller
    {
        [AuthRequire]
        public IActionResult Index()
        {
            return View();
        }

        [AuthRequire]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [AuthRequire]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult LoginUser(User aUser)
        {
            if (aUser.UserName == "admin" && aUser.Password == "dark.Invent")
            {
                HttpContext.Session.Set("login", Encoding.ASCII.GetBytes(DateTime.Now.ToString()));
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("Error", "Invalid username and/or password.");
            return View("Login", aUser);
        }

        [AllowAnonymous]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index");
        }
    }
}
