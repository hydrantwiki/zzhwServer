using System;
using System.Linq;
using HydrantWiki.Library.Managers;
using Nancy;
using Nancy.Responses.Negotiation;
using Nancy.Security;
using Site.Helpers;
using TreeGecko.Library.Common.Helpers;
using HydrantWiki.Library.Objects;
using TreeGecko.Library.Net.Objects;

namespace Site.Modules
{
    public class AppModule : NancyModule
    {
        public AppModule()
        {
            this.RequiresAuthentication();
           
            Get["/home"] = _parameters =>
            {
                return View["home.sshtml"];
            };

            Get["/mytags"] = _parameters =>
            {
                return View["mytags.sshtml"];
            };

            Get["/hydrants"] = _parameters =>
            {
                return View["hydrants.sshtml"];
            };

            Get["/changepassword"] = _parameters =>
            {
                return View["changepassword.sshtml"];
            };

            Post["/changepassword"] = _parameters =>
            {
                Response response = HandleChangePassword(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Get["/downloaddata"] = _parameters =>
            {
                return View["downloaddata.sshtml"];
            };

            Get["/map/tag/{Guid}"] = _parameters =>
            {
                return GetTagMap(_parameters);
            };


            Get["/reviewtags"] = _parameters =>
            {
                if (Context.CurrentUser.HasClaim("SuperUser")
                    || Context.CurrentUser.HasClaim("Admin"))
                {
                    return View["reviewtags.sshtml"];
                }
                else
                {
                    return View["home.sshtml"];
                }
            };

            Get["/reviewtag/{Guid}"] = _parameters =>
            {
                if (Context.CurrentUser.HasClaim("SuperUser")
                    || Context.CurrentUser.HasClaim("Admin"))
                {
                    if (_parameters != null
                        && _parameters.ContainsKey("Guid")
                        && GuidHelper.IsValidGuidString(_parameters["Guid"]))
                    {
                        Guid tagGuid = new Guid(_parameters["Guid"]);

                        HydrantWikiManager hwm = new HydrantWikiManager();
                        Tag tag = hwm.GetTag(tagGuid);

                        if (tag != null)
                        {
                            return View["reviewtag.sshtml", tag];
                        }
                    }
                    return null;
                }
                else
                {
                    return View["home.sshtml"];
                }
            };
        }

        private string HandleChangePassword(DynamicDictionary _parameters)
        {
            string currentPassword = Request.Form["currentpassword"];
            string newPassword = Request.Form["newpassword"];
            string username = Request.Headers["Username"].First();

            User user = null;
            AuthHelper.Authorize(username, currentPassword, out user);

            if (user != null)
            {
                TGUserPassword up = TGUserPassword.GetNew(user.Guid, user.Username, newPassword);

                HydrantWikiManager hwm = new HydrantWikiManager();
                hwm.Persist(up);

                return @"{ ""Result"":""Success"" }"; ;
            }

            return @"{ ""Result"":""Failure"" }"; 
        }

        private Negotiator GetTagMap(DynamicDictionary _parameters)
        {
            if (_parameters != null
                && _parameters.ContainsKey("Guid")
                && GuidHelper.IsValidGuidString(_parameters["Guid"]))
            {
                Guid tagGuid = new Guid(_parameters["Guid"]);

                HydrantWikiManager hwm = new HydrantWikiManager();

                Tag tag = hwm.GetTag(tagGuid);

                return View["popupmap.sshtml", tag];
            }

            return null;
        }
        
    }
}