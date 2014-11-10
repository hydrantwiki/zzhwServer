using System;
using System.Linq;
using System.Text;
using HydrantWiki.Library.Constants;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using Nancy;
using TreeGecko.Library.Net.Objects;

namespace Site.Helpers
{
    public static class AuthHelper
    {
        public static bool IsAuthorized(Request _request, out User _user)
        {
            HydrantWikiManager hwManager = new HydrantWikiManager();

            string username = _request.Headers["Username"].First();
            string authToken = _request.Headers["AuthorizationToken"].First();

            User user = hwManager.GetUser(UserSources.HydrantWiki, username);
            if (user != null)
            {
                TGUserAuthorization userAuth = hwManager.GetUserAuthorization(user.Guid, authToken);

                if (userAuth != null
                    && !userAuth.IsExpired())
                {
                    _user = user;

                    return true;
                }
            }

            _user = null;
            return false;
        }

        public static string Authorize(string _username, string _password, out User _user)
        {
            HydrantWikiManager hwManager = new HydrantWikiManager();
            _user = hwManager.GetUser(UserSources.HydrantWiki, _username);

            if (_user != null)
            {
                if (_user.IsVerified)
                {
                    if (_user.Active)
                    {
                        if (hwManager.ValidateUser(_user, _password))
                        {
                            TGUserAuthorization authorization =
                                TGUserAuthorization.GetNew(_user.Guid, "unknown");
                            hwManager.Persist(authorization);

                            //Done with a string builder to avoid the json braces that confuse string.format
                            StringBuilder sb = new StringBuilder();
                            sb.Append("{ \"Result\":\"Success\", \"AuthorizationToken\":\"");
                            sb.Append(authorization.AuthorizationToken);
                            sb.Append("\", \"DisplayName\":\"");
                            sb.Append(_user.DisplayName);
                            sb.Append("\", \"UserName\":\"");
                            sb.Append(_user.Username);
                            sb.Append("\" }");

                            return sb.ToString();
                        }

                        //Bad password or username
                        hwManager.LogWarning(Guid.Empty, "User not found");
                        _user = null;
                        return @"{ ""Result"":""BadUserOrPassword"" }";
                    }

                    //user not active
                    //Todo - Log Something
                    hwManager.LogWarning(_user.Guid, "User Not Active");
                    _user = null;
                    return @"{ ""Result"":""NotActive"" }";
                }

                //User not verified
                //Todo - Log Something
                hwManager.LogWarning(_user.Guid, "User not verified");
                _user = null;
                return @"{ ""Result"":""NotVerified"" }";
            }

            //User not found
            hwManager.LogWarning(Guid.Empty, "User not found");
            _user = null;
            return @"{ ""Result"":""BadUserOrPassword"" }";
        }

        public static string Authorize(Request _request, out User _user)
        {
            string username = _request.Headers["Username"].First();
            string password = _request.Headers["Password"].First();

            return Authorize(username, password, out _user);
        }
    }
}