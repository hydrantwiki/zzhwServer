using System;
using System.Collections.Generic;
using System.Linq;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using Nancy;
using Newtonsoft.Json;
using Site.Helpers;
using Site.JsonObjects;
using Site.Objects;
using TreeGecko.Library.Common.Helpers;
using TreeGecko.Library.Geospatial.Objects;
using Tag = HydrantWiki.Library.Objects.Tag;

namespace Site.RestModules
{
    public class TagModule : NancyModule
    {
        public TagModule()
        {
            Get["/rest/tag"] = _parameters =>
            {
                Response response = @"{ ""Result"":""Post Only"" }";
                response.ContentType = "application/json";
                return response;
            };

            Post["/rest/tag"] = _parameters =>
            {
                Response response = (Response) HandlePost(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Get["/rest/tags/table"] = _parameters =>
            {
                Response response = (Response)HandleGetMyTagsInTable(_parameters);
                response.ContentType = "application/json";
                return response;
            };
        }

        private string HandleGetMyTagsInTable(DynamicDictionary _parameters)
        {
            User user;

            if (AuthHelper.IsAuthorized(Request, out user))
            {
                HydrantWikiManager hwManager = new HydrantWikiManager();
                List<Tag> tags = hwManager.GetTagsForUser(user.Guid);

                TagTableResponse response = new TagTableResponse {Result = "Success"};

                foreach (Tag tag in tags)
                {
                    JsonObjects.Tag jTag = new JsonObjects.Tag(tag);
                    response.Data.Add(jTag);
                }

                string json = JsonConvert.SerializeObject(response);

                return json;
            }

            return null;
        }

        private string HandlePost(DynamicDictionary _parameters)
        {
            User user;

            if (AuthHelper.IsAuthorized(Request, out user))
            {
                HydrantWikiManager hwManager = new HydrantWikiManager();

                string sLatitude = Request.Form["latitudeInput"];
                string sLongitude = Request.Form["longitudeInput"];
                string sAccuracy = Request.Form["accuracyInput"];
                string sDeviceDateTime = Request.Form["positionDateTimeInput"];

                double lat = 0.0;
                double lon = 0.0;
                double accuracy = -1;

                DateTime deviceDateTime = DateTime.MinValue;
                GeoPoint geoPoint = null;

                if (Double.TryParse(sLatitude, out lat))
                {
                    if (Double.TryParse(sLongitude, out lon))
                    {
                        //Ignore positions that are 0.0 and 0.0 exactly
                        if (!(lat.Equals(0)
                              && lon.Equals(0)))
                        {
                            geoPoint = new GeoPoint {X = lon, Y = lat};
                        }
                    }
                }

                Double.TryParse(sAccuracy, out accuracy);
                DateTime.TryParse(sDeviceDateTime, out deviceDateTime);

                //If we got a timestamp that was a zero date, ignore it and use now.
                if (deviceDateTime == DateTime.MinValue)
                {
                    deviceDateTime = DateTime.UtcNow;
                }

                //We will accept a tag without a photo, but not one without a position.
                if (geoPoint != null)
                {
                    Tag tag = new Tag
                    {
                        Active = true,
                        DeviceDateTime = deviceDateTime,
                        LastModifiedDateTime = DateTime.UtcNow,
                        UserGuid = user.Guid,
                        VersionTimeStamp = DateTime.UtcNow.ToString("u"),
                        Position = geoPoint
                    };

                    if (Request.Files.Any())
                    {
                        tag.ImageGuid = Guid.NewGuid();
                    }
                    else
                    {
                        tag.ImageGuid = null;
                    }

                    try
                    {
                        if (tag.ImageGuid != null)
                        {
                            HttpFile file = Request.Files.First();

                            long fileSize = file.Value.Length;

                            try
                            {
                                byte[] data = new byte[fileSize];

                                file.Value.Read(data, 0, (int)fileSize);
                               
                                hwManager.PersistOriginal(tag.ImageGuid.Value, ".jpg", "image/jpg", data);

                                hwManager.LogVerbose(user.Guid, "Tag Image Saved");

                                return @"{ ""Result"":""Success"" }"; 
                            }
                            catch (Exception ex)
                            {
                                hwManager.LogException(user.Guid, ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        hwManager.LogException(user.Guid, ex);
                    }
                }
                else
                {
                    //No position
                    hwManager.LogWarning(user.Guid, "No position");

                    return @"{ ""Result"":""Failure - No position"" }";
                }
            }

            return @"{ ""Result"":""Failure"" }";
        }
    }
}