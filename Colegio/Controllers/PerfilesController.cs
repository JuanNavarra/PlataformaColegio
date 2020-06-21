using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Colegio.Controllers
{
    public class PerfilesController : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    return View();
                }
                return Redirect("~/Login/Authentication");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
