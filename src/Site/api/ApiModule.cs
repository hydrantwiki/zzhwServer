using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using HydrantWiki.Library.Constants;
using HydrantWiki.Library.Helpers;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using Nancy;
using Newtonsoft.Json;
using Site.api.Objects;
using Site.api.Objects.Responses;
using Site.Extensions;
using Site.Helpers;
using TreeGecko.Library.Common.Helpers;
using TreeGecko.Library.Geospatial.Helpers;
using TreeGecko.Library.Geospatial.Objects;
using TreeGecko.Library.Net.Objects;
using NearbyHydrant = Site.api.Objects.HydrantHeader;

namespace Site.api
{
    public class ApiModule: NancyModule
    {
        public ApiModule()
        {
            Get["/api/authorize"] = _parameters =>
            {
                Response response = @"{ ""Result"":""Post Only"" }";
                response.ContentType = "application/json";
                return response;
            };

            Post["/api/authorize"] = _parameters =>
            {
                Response response = (Response)Authorize(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Get["/api/user/isavailable/{username}"] = _parameters =>
            {
                Response response = (Response)IsAvailable(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Get["/api/tags/count"] = _parameters =>
            {
                Response response = (Response)HangleGetTagCount(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Get["/api/tags/mine/count"] = _parameters =>
            {
                Response response = (Response)HandleGetMyTagCount(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Post["/api/tag"] = _parameters =>
            {
                Response response = (Response) HandleTagPost(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Post["/api/image/{fileName}"] = _parameters =>
            {
                Response response = (Response)HandleImagePost(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Post["/api/image"] = _parameters =>
            {
                Response response = (Response)HandleImagePost(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Get["/api/hydrants/{east}/{west}/{north}/{south}"] = _parameters =>
            {
                Response response = HangleGetHydrantsByGeobox(_parameters);
                response.ContentType = "application/json"; ;
                return response;
            };

            Get["/api/hydrants/{east}/{west}/{north}/{south}/{quantity}"] = _parameters =>
            {
                Response response = HangleGetHydrantsByGeobox(_parameters);
                response.ContentType = "application/json"; ;
                return response;
            };

            Get["/api/hydrants/{latitude}/{longitude}/{distance}"] = _parameters =>
            {
                Response response = HangleGetHydrantsByCenterDistance(_parameters);
                response.ContentType = "application/json";
                return response;
            };
        }

        private string HangleGetTagCount(DynamicDictionary _parameters)
        {
            User user;
            BaseResponse response;

            if (AuthHelper.IsAuthorized(Request, out user))
            {
                HydrantWikiManager hwm = new HydrantWikiManager();
                int count = hwm.GetTagCount();

                response = new TagCountResponse(true, count);
            }
            else
            {
                response = new BaseResponse {Success = false};
            }

            return JsonConvert.SerializeObject(response);
        }

        private string HandleGetMyTagCount(DynamicDictionary _parameters)
        {
            User user;
            BaseResponse response;
            
            if (AuthHelper.IsAuthorized(Request, out user))
            {
                HydrantWikiManager hwm = new HydrantWikiManager();
                int count = hwm.GetTagCount(user.Guid);

                response = new TagCountResponse(true, count);
            }
            else
            {
                response = new BaseResponse {Success = false};
            }

            string json = JsonConvert.SerializeObject(response);
            return json;
        }

        private string Authorize(DynamicDictionary _parameters)
        {
            User user;
            string result = AuthHelper.Authorize(Request, out user);
            return result;
        }

        private string IsAvailable(DynamicDictionary _parameters)
        {
            HydrantWikiManager hwm = new HydrantWikiManager();

            string username = _parameters["username"];
            User user = hwm.GetUser(UserSources.HydrantWiki, username);

            if (user == null)
            {
                return "{ \"Available\":true }";
            }

            return "{ \"Available\":false }";
        }

        private string HandleImagePost(DynamicDictionary _parameters)
        {
            User user;

            if (AuthHelper.IsAuthorized(Request, out user))
            {
                HydrantWikiManager hwManager = new HydrantWikiManager();

                byte[] fileData = null;
                string fileName = null;

                if (Request.Files.Any())
                {
                    HttpFile file = Request.Files.First();

                    long length = file.Value.Length;
                    fileData = new byte[(int) length];
                    file.Value.Read(fileData, 0, (int) length);
                    fileName = file.Name;
                }

                if (fileName != null)
                {
                    string tempGuid = Path.GetFileNameWithoutExtension(fileName);
                    Guid imageGuid;

                    if (Guid.TryParse(tempGuid, out imageGuid))
                    {
                        try
                        {
                            hwManager.PersistOriginal(imageGuid, ".jpg", "image/jpg", fileData);
                            hwManager.LogVerbose(user.Guid, "Tag Image Saved");

                            Image original = ImageHelper.GetImage(fileData);

                            fileData = ImageHelper.GetThumbnailBytesOfMaxSize(original, 800);
                            hwManager.PersistWebImage(imageGuid, ".jpg", "image/jpg", fileData);

                            fileData = ImageHelper.GetThumbnailBytesOfMaxSize(original, 100);
                            hwManager.PersistThumbnailImage(imageGuid, ".jpg", "image/jpg", fileData);

                            return @"{ ""Success"":true }";
                        }
                        catch (Exception ex)
                        {
                            hwManager.LogException(user.Guid, ex);
                        }
                    }
                }
            }

            return @"{ ""Success"":false }";
        }

        private string HandleTagPost(DynamicDictionary _parameters)
        {
            User user;

            if (AuthHelper.IsAuthorized(Request, out user))
            {
                HydrantWikiManager hwManager = new HydrantWikiManager();

                string json = Request.Body.ReadAsString();

                Objects.Tag tag = JsonConvert.DeserializeObject<Objects.Tag>(json);

                if (tag != null)
                {
                    if (tag.Position != null)
                    {
                        if (tag.Position != null)
                        {
                            var dbTag = new HydrantWiki.Library.Objects.Tag
                            {
                                Active = true,
                                DeviceDateTime = tag.Position.DeviceDateTime,
                                LastModifiedDateTime = DateTime.UtcNow,
                                UserGuid = user.Guid,
                                VersionTimeStamp = DateTime.UtcNow.ToString("u"),
                                Position = new GeoPoint(tag.Position.Longitude, tag.Position.Latitude),
                                ImageGuid = tag.ImageGuid,
                                Status = TagStatus.Pending
                            };

                            try
                            {
                                hwManager.Persist(dbTag);
                                hwManager.LogVerbose(user.Guid, "Tag Saved");                               

                                return @"{ ""Success"":true }";
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

                            return @"{ ""Success"":false, ""Message"": ""No position"" }";
                        }
                    }
                }
            }

            return @"{ ""Success"":false }";
        }

        public string HangleGetHydrantsByCenterDistance(DynamicDictionary _parameters)
        {
            double latitude = Convert.ToDouble((string)_parameters["latitude"]);
            double longitude = Convert.ToDouble((string)_parameters["longitude"]);
            double distance = Convert.ToDouble((string)_parameters["distance"]);

            HydrantWikiManager hwm = new HydrantWikiManager();
            GeoPoint center = new GeoPoint(longitude, latitude);

            List<Hydrant> hydrants = hwm.GetHydrants(center, distance);

            List<HydrantHeader> headers = ProcessHydrants(hydrants, center);

            HydrantQueryResponse response = new HydrantQueryResponse {Success = true, Hydrants = headers};

            return JsonConvert.SerializeObject(response);
        }

        public string HangleGetHydrantsByGeobox(DynamicDictionary _parameters)
        {
            HydrantWikiManager hwm = new HydrantWikiManager();

            double east = Convert.ToDouble((string)_parameters["east"]);
            double west = Convert.ToDouble((string)_parameters["west"]);
            double north = Convert.ToDouble((string)_parameters["north"]);
            double south = Convert.ToDouble((string)_parameters["south"]);

            int quantity = 250;
            if (_parameters.ContainsKey("quantity"))
            {
                quantity = Convert.ToInt32((string) _parameters["quantity"]);
            }
            if (quantity > 500)
            {
                quantity = 500;
            }

            GeoBox geobox = new GeoBox(east, west, north, south);

            List<Hydrant> hydrants = hwm.GetHydrants(geobox, quantity);

            List<HydrantHeader> headers = ProcessHydrants(hydrants);

            HydrantQueryResponse response = new HydrantQueryResponse { Success = true, Hydrants = headers };

            return JsonConvert.SerializeObject(response);
        }

        private List<HydrantHeader> ProcessHydrants(IEnumerable<Hydrant> _hydrants, GeoPoint _center = null)
        {
            HydrantWikiManager hwm = new HydrantWikiManager();
            Dictionary<Guid, string> users = new Dictionary<Guid, string>();

            var output = new List<HydrantHeader>();
            foreach (var hydrant in _hydrants)
            {
                string username;
                Guid userGuid = hydrant.OriginalTagUserGuid;

                if (users.ContainsKey(userGuid))
                {
                    username = users[userGuid];
                }
                else
                {
                    TGUser user = hwm.GetUser(userGuid);
                    users.Add(user.Guid, user.Username);
                    username = user.Username;
                }

                var outputHydrant = new HydrantHeader
                {
                    HydrantGuid = hydrant.Guid,
                    Position = new GeoLocation(hydrant.Position.Y, hydrant.Position.X, 0),
                    ThumbnailUrl = hydrant.ThumbnailUrl,
                    ImageUrl = hydrant.ImageUrl,
                    Username = username
                };

                if (_center == null)
                {
                    outputHydrant.DistanceInFeet = null;
                }
                else
                {
                    outputHydrant.DistanceInFeet = PositionHelper.GetDistance(_center, hydrant.Position).ToFeet();
                }

                output.Add(outputHydrant);
            }

            return output;
        }
    }
}