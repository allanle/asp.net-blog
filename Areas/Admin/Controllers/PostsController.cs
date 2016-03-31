using asp.net_blog.Infrastructure;
using asp.net_blog.Models;
using asp.net_blog.Areas.Admin.ViewModels;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate.Linq;

namespace asp.net_blog.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [SelectedTab("posts")]
    public class PostsController : Controller
    {
        private const int PostsPerPage = 5;

        public ActionResult Index(int page = 1)
        {
            var totalPostCount = Database.Session.Query<Post>().Count();

            // Grab Posts by descending order
            var currentPostPage = Database.Session.Query<Post>()
                .OrderByDescending(c => c.CreateAt)
                .Skip((page - 1) * PostsPerPage)
                .Take(PostsPerPage)
                .ToList();

            return View(new PostsIndex
            {
                Posts = new PagedData<Post>(currentPostPage, totalPostCount, page, PostsPerPage)
            });
        }
    }
}