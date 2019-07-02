using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Controllers
{
    public class BlankController : Controller
    {
        public IActionResult ChangeLog()
        {
            return View();
        }
    }
}
