using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmashPopMVC.Data.Sync;
//using SmashPopMVC.Data.Async;
using SmashPopMVC.Models;

namespace SmashPopMVC.Areas.Shared.Controllers
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
