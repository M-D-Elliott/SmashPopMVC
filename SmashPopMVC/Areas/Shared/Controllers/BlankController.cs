using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Areas.Shared.Controllers
{
    public class BlankController : Controller
    {
        [RequireHttps, AllowAnonymous]
        public IActionResult ChangeLog()
        {
            return View();
        }
    }
}
