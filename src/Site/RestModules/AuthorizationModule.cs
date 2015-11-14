using System.Collections.Specialized;
using System.Linq;
using HydrantWiki.Library.Constants;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using Nancy;
using Site.Helpers;
using TreeGecko.Library.Common.Enums;
using TreeGecko.Library.Net.Objects;

namespace Site.RestModules
{
    public class AuthorizationModule : NancyModule
    {
        public AuthorizationModule()
        {
            Get["/rest/authorize"] = _parameters =>
            {
                Response response = @"{ ""Result"":""Post Only"" }";
                response.ContentType = "application/json";
                return response;
            };

            Post["/rest/authorize"] = _parameters =>
            {
                Response response = (Response)Authorize(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Get["/rest/user/isavailable/{username}"] = _parameters =>
            {
                Response response = (Response)IsAvailable(_parameters);
                response.ContentType = "application/json";
                return response;
            };
        }
       
        private string Authorize(DynamicDictionary _parameters)
        {
            User user;
            string result = AuthHelper.Authorize(Request, out user);
            return result;
        }

        private string IsAvailable(DynamicDictionary _parameters)
        {
            string username = _parameters["username"];

            HydrantWikiManager hwm = new HydrantWikiManager();

            User user = hwm.GetUser(UserSources.HydrantWiki, username);

            if (user == null)
            {
                return "{ \"Available\":true }";
            }
            
            return "{ \"Available\":false }";
        }


    }
}