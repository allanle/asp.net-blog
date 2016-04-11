﻿using asp.net_blog.Infrastructure;
using asp.net_blog.Models;
using asp.net_blog.Areas.Admin.ViewModels;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using asp.net_blog.Infrastructure.Extensions;

namespace asp.net_blog.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [SelectedTab("posts")]
    public class PostsController : Controller
    {
        private const int PostsPerPage = 10;

        public ActionResult Index(int page = 1)
        {
            var totalPostCount = Database.Session.Query<Post>().Count();

            var postIds = Database.Session.Query<Post>()
                .OrderByDescending(c => c.CreateAt)
                .Skip((page - 1) * PostsPerPage)
                .Take(PostsPerPage)
                .Select(p => p.Id)
                .ToArray();

            // Grab Posts by descending order
            var currentPostPage = Database.Session.Query<Post>()
                .Where(p => postIds.Contains(p.Id))
                .OrderByDescending(c => c.CreateAt)
                .FetchMany(f => f.Tags)
                .Fetch(f => f.user)
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
                IsNew = true,
                Tags = Database.Session.Query<Tag>().Select(tag => new TagCheckbox
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    IsChecked = false
                }).ToList()
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
                Title = post.Title,
                Tags = Database.Session.Query<Tag>().Select(tag => new TagCheckbox
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    IsChecked = post.Tags.Contains(tag)
                }).ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Form(PostsForm form)
        {
            form.IsNew = form.PostId == null;

            if(!ModelState.IsValid)
            {
                return View(form);
            }

            var selectedTags = ReconsileTags(form.Tags);

            Post post;
            if (form.IsNew)
            {
                post = new Post
                {
                    CreateAt = DateTime.UtcNow.ToLocalTime(),
                    user = Auth.User,
                };

                foreach (var tag in selectedTags)
                {
                    post.Tags.Add(tag);
                }
            }
            else
            {
                post = Database.Session.Load<Post>(form.PostId);

                if (post == null)
                {
                    return HttpNotFound();
                }
                post.UpdatedAt = DateTime.UtcNow.ToLocalTime();

                foreach (var toAdd in selectedTags.Where(t => !post.Tags.Contains(t)))
                {
                    post.Tags.Add(toAdd);
                }

                foreach (var toRemove in post.Tags.Where(t => !selectedTags.Contains(t)).ToList())
                {
                    post.Tags.Remove(toRemove);
                }
            }

            post.Title = form.Title;
            post.Slug = form.Slug;
            post.Content = form.Content;

            Database.Session.SaveOrUpdate(post);

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Trash(int id)
        {
            var post = Database.Session.Load<Post>(id);

            if(post == null)
            {
                return HttpNotFound();
            }

            post.DeletedAt = DateTime.UtcNow.ToLocalTime();
            Database.Session.Update(post);

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var post = Database.Session.Load<Post>(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            Database.Session.Delete(post);

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Restore(int id)
        {
            var post = Database.Session.Load<Post>(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            post.DeletedAt = null;
            Database.Session.Update(post);

            return RedirectToAction("Index");
        }

        private IEnumerable<Tag> ReconsileTags(IEnumerable<TagCheckbox> tags)
        {
            foreach(var tag in tags.Where(t => t.IsChecked))
            {
                if(tag.Id != null)
                {
                    yield return Database.Session.Load<Tag>(tag.Id);
                    continue;
                }

                var existingTag = Database.Session.Query<Tag>().FirstOrDefault(t => t.Name == tag.Name);

                if(existingTag != null)
                {
                    yield return existingTag;
                    continue;
                }

                var newTag = new Tag
                {
                    Name = tag.Name,
                    Slug = tag.Name.Slugify()
                };

                Database.Session.Save(newTag);
                yield return newTag;
            }
        }
    }
}