using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Class_Relax.Controllers
{


    [AllowAnonymous]
    public class HomeController : Controller
    {
        
        [AllowAnonymous]
        public ActionResult Default(string message)
        {
            if (message == null)
            {
                return View();
            }
            return View((Object)message);
        }



    }
}