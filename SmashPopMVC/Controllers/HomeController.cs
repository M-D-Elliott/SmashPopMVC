using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmashPopMVC.Data;
using SmashPopMVC.Models;

namespace SmashPopMVC.Controllers
{
    public class HomeController : Controller
    {

        private readonly ICharacter _characterService;

        public HomeController(ICharacter characterService)
        {
            _characterService = characterService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var characters = _characterService.GetByID(9);
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Profile", "ApplicationUser");
            }
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
