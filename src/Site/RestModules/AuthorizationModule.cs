using HydrantWiki.Library.Objects;
using Nancy;
using Site.Helpers;

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
                Response response = (Response)HandlePost(_parameters);
                response.ContentType = "application/json";
                return response;
            };
        }

        private string HandlePost(DynamicDictionary _parameters)
        {
            User user;

            string result = AuthHelper.Authorize(Request, out user);

            return result;
        }

    }
}