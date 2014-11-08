using System;
using System.Text;
using System.Web;
using System.Web.SessionState;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using TreeGecko.Library.Net.Handlers;
using TreeGecko.Library.Net.Helpers;
using TreeGecko.Library.Net.Objects;

namespace HydrantWiki.Web.Handlers
{
    /// <summary>
    /// Summary description for Authorization
    /// </summary>
    public class Authorization : AbstractHandler, IRequiresSessionState
    {
        public override void HandleGet(HttpContext _context)
        {
            HydrantWikiManager hwManager = new HydrantWikiManager();
            _context.Response.ContentType = "application/json";

            HttpRequest request = _context.Request;
            string userSource = request.Headers["UserSource"];
            string username = request.Headers["Username"];
            string password = request.Headers["Password"];

            if (string.IsNullOrEmpty(userSource))
            {
                userSource = "HydrantWiki";
            }

            User user = hwManager.GetUser(userSource, username);

            if (user != null)
            {
                if (user.IsVerified)
                {
                    if (user.Active)
                    {
                        if (hwManager.ValidateUser(user, password))
                        {
                            TGUserAuthorization authorization =
                                TGUserAuthorization.GetNew(user.Guid, BrowserHelper.GetBrowserType(request));
                            hwManager.Persist(authorization);

                            _context.Session["User"] = user;

                            //Done with a string builder to avoid the json braces that confuse string.format
                            StringBuilder sb = new StringBuilder();
                            sb.Append("{ \"Result\":\"Success\", \"AuthorizationToken\":\"");
                            sb.Append(authorization.AuthorizationToken);
                            sb.Append("\", \"DisplayName\":\"");
                            sb.Append(user.DisplayName);
                            sb.Append("\", \"UserName\":\"");
                            sb.Append(user.Username);
                            sb.Append("\" }");

                            _context.Response.Write(sb.ToString());
                        }
                        else
                        {
                            _context.Response.Write(@"{ ""Result"":""BadUserOrPassword"" }");
                            hwManager.LogWarning(Guid.Empty, "User not found");
                        }
                    }
                    else
                    {
                        //user not active
                        //Todo - Log Something

                        _context.Response.Write(@"{ ""Result"":""NotActive"" }");

                        hwManager.LogWarning(user.Guid, "User Not Active");
                    }
                }
                else
                {
                    //User not verified
                    //Todo - Log Something

                    _context.Response.Write(@"{ ""Result"":""NotVerified"" }");
                    hwManager.LogWarning(user.Guid, "User not verified");
                }
            }
            else
            {
                _context.Response.Write(@"{ ""Result"":""BadUserOrPassword"" }");
                hwManager.LogWarning(Guid.Empty, "User not found");
            }
        }
    }
}