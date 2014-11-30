using System;
using HydrantWiki.Library.Constants;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using Nancy;
using Nancy.Responses.Negotiation;
using Site.Helpers;
using TreeGecko.Library.Common.Enums;
using TreeGecko.Library.Common.Helpers;
using TreeGecko.Library.Geospatial.Objects;
using Tag = HydrantWiki.Library.Objects.Tag;

namespace Site.RestModules
{
    public class YoModule: NancyModule
    {
        public YoModule()
        {
            Get["/rest/yotagcallback"] = _parameters =>
            {
                CreateYoTag(_parameters);

                Response response = @"{ ""Result"":""Success"" }";
                response.ContentType = "application/json";
                return response;
            };

            Get["/rest/yotag/{TagGuid}"] = _parameters =>
            {
                string sGuid = _parameters["TagGuid"];

                Guid tagGuid;

                if (Guid.TryParse(sGuid, out tagGuid))
                {
                    HydrantWikiManager hwm = new HydrantWikiManager();
                    Tag tag = hwm.GetTag(tagGuid);

                    Context.ViewBag.MapBoxMap = Config.GetSettingValue("MapBoxMap");
                    Context.ViewBag.MapBoxKey = Config.GetSettingValue("MapBoxKey");

                    return View["yotag.sshtml", tag];
                }

                return null;
            };

            Post["/rest/yotag/{TagGuid}"] = _parameters =>
            {
                return null;
            };
        }

        public string CreateYoTag(DynamicDictionary _parameters)
        {
            string username = Request.Query["username"];
            string location = Request.Query["location"];

            if (username != null
                && location != null)
            {
                GeoPoint geoPoint = null;
                string[] locationParams = location.Split(";".ToCharArray());
                if (locationParams.Length > 1)
                {
                    geoPoint = new GeoPoint(Convert.ToDouble(locationParams[1]),
                        Convert.ToDouble(locationParams[0]));    
                }

                if (geoPoint != null)
                {
                    HydrantWikiManager hwm = new HydrantWikiManager();
                    User user = hwm.GetUser(UserSources.Yo, username);

                    if (user == null)
                    {
                        user = new User
                        {
                            UserSource = UserSources.Yo, 
                            UserType = UserTypes.User, 
                            Username = username,
                            Active = true
                        };

                        hwm.Persist(user);
                    }

                    Tag tag = new Tag
                    {
                        UserGuid = user.Guid,
                        Status = TagStatus.Pending,
                        DeviceDateTime = DateTime.UtcNow,
                        ExternalIdentifier = null,
                        ExternalSource = null,
                        Position = geoPoint
                    };

                    hwm.Persist(tag);

                    string url = Config.GetSettingValue("SystemUrl") + string.Format("/rest/yotag/{0}", tag.Guid);

                    YoHelper.SendYo(username, url);
                }
            }

            return "";
        }
    }
}