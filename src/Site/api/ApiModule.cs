using System;
using System.Drawing;
using System.IO;
using System.Linq;
using HydrantWiki.Library.Constants;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using Nancy;
using Newtonsoft.Json;
using Site.api.Objects;
using Site.api.Objects.Responses;
using Site.Extensions;
using Site.Helpers;
using TreeGecko.Library.Common.Helpers;
using TreeGecko.Library.Geospatial.Objects;
using Tag = HydrantWiki.Library.Objects.Tag;

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
                response = new BaseResponse();
                response.Success = false;
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
                response = new BaseResponse();
                response.Success = false;
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
                            Tag dbTag = new Tag
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

    }
}