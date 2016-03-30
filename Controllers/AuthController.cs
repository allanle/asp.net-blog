using asp.net_blog.Models;
using asp.net_blog.ViewModels;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace asp.net_blog.Controllers
{
    public class AuthController : Controller
    {
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToRoute("home");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(AuthLogin form, string returnUrl)
        {
            var user = Database.Session.Query<User>().FirstOrDefault(u => u.Username == form.Username);

            //  Prevent timing attacks. Attackers won't be able to detemine the amout of ms when in console.
            if(user == null)
            {
                asp.net_blog.Models.User.FakeHash();
            }

            if (user == null || !user.CheckPassword(form.Password))
            {
                ModelState.AddModelError("Username", "Username or password is incorrect");
            }

            if(!ModelState.IsValid)
            {
                return View(form);
            }


            FormsAuthentication.SetAuthCookie(user.Username, true);
            
            if(!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToRoute("home");
        }
    }    
}