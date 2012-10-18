using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace e3Radio.Data
{
    class UserManager
    {
        public static User GetOrCreateUser(E3RadioEntities db, long userId)
        {
            var u = db.Users.SingleOrDefault(us => us.UserID == userId);
            if (u == null)
            {
                u = new e3Radio.Data.User();

                // get the dude's info from book of face
                //var fb = new Facebook.Web.FacebookWebClient();
                //dynamic me = fb.Get("/me");
                //u.UserID = userId;
                //u.Username = me.username ?? me.name;
                //u.Name = me.name;
                //u.FacebookLink = me.link;
                //u.DateCreated = DateTime.Now;

                db.Users.Add(u);
                db.SaveChanges();
            }
            return u;
        }
    }
}
