using asp.net_blog.Models;
using asp.net_blog.ViewModels;
using NHibernate.Linq;
using System.Linq;
using System.Web.Mvc;

namespace asp.net_blog.Controllers
{
    public class LayoutController : Controller
    {
        // GET: Layout
        [ChildActionOnly]
        public ActionResult Sidebar()
        {
            return View(new LayoutSidebar
            {
                IsLoggedIn = Auth.User != null,
                Username = Auth.User != null ? Auth.User.Username : "",
                IsAdmin = User.IsInRole("admin"),
                Tags = Database.Session.Query<Tag>().Select(tag => new
                {
                    tag.Id,
                    tag.Name,
                    tag.Slug,
                    PostCount = tag.Posts.Count
                }).Where(t => t.PostCount > 0).OrderByDescending(p => p.PostCount).Select(
                    tag => new SidebarTag(tag.Id, tag.Name, tag.Slug, tag.PostCount)).ToList()
            });
        }
    }
}