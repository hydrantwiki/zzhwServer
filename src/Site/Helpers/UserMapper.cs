using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;
using Site.Objects;
using TreeGecko.Library.Common.Enums;

namespace Site.Helpers
{
    public class UserMapper : IUserMapper
    {
        public IUserIdentity GetUserFromIdentifier(Guid _identifier, NancyContext _context)
        {
            HydrantWikiManager hwm = new HydrantWikiManager();

            User user = (User)hwm.GetUser(_identifier);

            if (user != null)
            {
                NancyUser nUser = new NancyUser {UserName = user.Username};
                nUser.SetClaims(user.UserType);
                return nUser;
            }

            return null;
        }
    }
}