using System.Linq;
using HydrantWiki.Library.Constants;
using HydrantWiki.Library.Managers;
using Nancy;
using Site.Helpers;
using TreeGecko.Library.Net.Objects;
using User = HydrantWiki.Library.Objects.User;

namespace Site.RestModules
{
    public class ReauthorizeModule : NancyModule
    {
        public ReauthorizeModule()
        {
            Get["/rest/reauthorize"] = _parameters =>
            {
                Response response = @"{ ""Result"":""Post Only"" }";
                response.ContentType = "application/json";
                return response;
            };

            Post["/rest/reauthorize"] = _parameters =>
            {
                Response response = (Response) HandleReauthroization(_parameters);
                response.ContentType = "application/json";
                return response;
            };
        }

        public string HandleReauthroization(DynamicDictionary _parameters)
        {
            User user;
            if (AuthHelper.IsAuthorized(Request, out user))
            {
                return @" { ""Result"":""Success"" } ";
            }
            
            return @" { ""Result"":""Failure"" } ";
        }
    }
}