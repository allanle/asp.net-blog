﻿using asp.net_blog.Infrastructure;
using asp.net_blog.Models;
using asp.net_blog.Areas.Admin.ViewModels;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate.Linq;
using System;

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

        public ActionResult New()
        {
            return View("Form", new PostsForm
            {
                IsNew = true
            });
        }

        public ActionResult Edit(int id)
        {
            var post = Database.Session.Load<Post>(id);

            if(post == null)
            {
                return HttpNotFound();
            }

            return View("Form", new PostsForm
            {
                IsNew = false,
                PostId = id,
                Content = post.Content,
                Slug = post.Slug,
                Title = post.Title
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Form(PostsForm form)
        {
            form.IsNew = form.PostId == null;

            if(!ModelState.IsValid)
            {
                return View(form);
            }

            Post post;
            if (form.IsNew)
            {
                post = new Post
                {
                    CreateAt = DateTime.UtcNow.ToLocalTime(),
                    user = Auth.User,
                };
            }
            else
            {
                post = Database.Session.Load<Post>(form.PostId);

                if(post == null)
                {
                    return HttpNotFound();
                }
                post.UpdatedAt = DateTime.UtcNow.ToLocalTime();
            }

            post.Title = form.Title;
            post.Slug = form.Slug;
            post.Content = form.Content;

            Database.Session.SaveOrUpdate(post);

            return RedirectToAction("Index");
        }
    }
}