using asp.net_blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace asp.net_blog.Areas.Admin.ViewModels
{
    public class UsersIndex
    {
        public IEnumerable<User> Users { get; set; }

    }
}