using asp.net_blog.Models;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace asp.net_blog
{
    public static class Auth
    {
        private const string UserKey = "asp.net_blog.Auth.UserKey";

        public static User User
        {
            get
            {
                // See if user is logged in
                if(!HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    return null;
                }
                var user = HttpContext.Current.Items[UserKey] as User;
                
                if(user == null)
                {
                    // Grab user from database.
                    user = Database.Session.Query<User>().FirstOrDefault(u => u.Username == HttpContext.Current.User.Identity.Name);

                    if(user == null)
                    {
                        return null;
                    }
                    HttpContext.Current.Items[UserKey] = user;
                }
                return user;
            }
        }
    }
}