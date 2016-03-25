using asp.net_blog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace asp.net_blog.Controllers
{
    public class AuthController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(AuthLogin form)
        {
            if(!ModelState.IsValid)
            {
                return View(form);
            }

            if(form.Username != "rainbow dash")
            {
                ModelState.AddModelError("Username", "Username or password isn't 20% cooler.");
                return View(form);
            }
            return Content("The form is valid!");
        }
    }    
}