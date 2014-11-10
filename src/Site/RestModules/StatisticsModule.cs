using System.Text;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using Nancy;
using Site.Helpers;

namespace Site.RestModules
{
    public class StatisticsModule : NancyModule
    {
        public StatisticsModule()
        {
            Get["/rest/statistics"] = _parameters =>
            {
                Response response = (Response)HandleGetStatistics(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Get["/rest/mystatistics"] = _parameters =>
            {
                Response response = (Response)HandleGetUserStatistics(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Get["/rest/userstatistics/{username}"] = _parameters =>
            {
                Response response = (Response)HandleGetUserStatistics(_parameters);
                response.ContentType = "application/json";
                return response;
            };
        }

        private string HandleGetMyStatistics(DynamicDictionary _parameters)
        {
            User user;

            if (AuthHelper.IsAuthorized(Request, out user))
            {
                long userHydrantCount = 0;
                long userTagCount = 0;

                StringBuilder sb = new StringBuilder();
                sb.Append(@"{ ""Result"":""Success"", ""UserHydrantCount"":""");
                sb.Append(userHydrantCount);
                sb.Append(@""", ""UserTagCount"":""");
                sb.Append(userTagCount);
                sb.Append(@""" }");

                return sb.ToString();
            }

            return @"{ ""Result"":""Failure"" }";
        }

        private string HandleGetUserStatistics(DynamicDictionary _parameters)
        {
            User user;

            if (AuthHelper.IsAuthorized(Request, out user))
            {
                long userHydrantCount = 0;
                long userTagCount =0;

                StringBuilder sb = new StringBuilder();
                sb.Append(@"{ ""Result"":""Success"", ""UserHydrantCount"":""");
                sb.Append(userHydrantCount);
                sb.Append(@""", ""UserTagCount"":""");
                sb.Append(userTagCount);
                sb.Append(@""" }");

                return sb.ToString();
            }

            return @"{ ""Result"":""Failure"" }";
        }

        private string HandleGetStatistics(DynamicDictionary _parameters)
        {
            User user;

            if (AuthHelper.IsAuthorized(Request, out user))
            {

                HydrantWikiManager hwManager = new HydrantWikiManager();

                long siteUserCount = hwManager.GetUserCount();
                long siteHydrantCount = hwManager.GetHydrantCount();
                long siteTagCount = hwManager.GetTagCount();

                StringBuilder sb = new StringBuilder();
                sb.Append(@"{ ""Result"":""Success"", ""SiteUserCount"":""");
                sb.Append(siteUserCount);
                sb.Append(@""", ""SiteHydrantCount"":""");
                sb.Append(siteHydrantCount);
                sb.Append(@""", ""SiteTagCount"":""");
                sb.Append(siteTagCount);
                sb.Append(@""" }");

                string temp = sb.ToString();

                return temp;

            }

            return @"{ ""Result"":""Failure"" }";
        }
    }
}