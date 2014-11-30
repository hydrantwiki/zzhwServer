using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HydrantWiki.Library.Constants;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using Nancy;
using Newtonsoft.Json;
using Site.Helpers;
using Site.JsonObjects;
using TreeGecko.Library.Common.Enums;
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

            Get["/rest/tags/mine/table"] = _parameters =>
            {
                Response response = (Response)HandleGetMyTagsInTable(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Get["/rest/tags/pending/table"] = _parameters =>
            {
                Response response = (Response)HandleGetPendingTagsInTable(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Post["/rest/tag/match/{tagGuid}/{hydrantGuid}"] = _parameters =>
            {
                Response response = (Response)MatchTagWithHydrant(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Post["/rest/tag/reject/{tagGuid}"] = _parameters =>
            {
                Response response = (Response)RejectTag(_parameters);
                response.ContentType = "application/json";
                return response;
            };

            Post["/rest/tag/accept/{tagGuid}"] = _parameters =>
            {
                Response response = (Response)AcceptTag(_parameters);
                response.ContentType = "application/json";
                return response;
            };
        }

        private string RejectTag(DynamicDictionary _parameters)
        {
            User user;

            if (AuthHelper.IsAuthorized(Request, out user))
            {
                if (user != null
                    && (user.UserType == UserTypes.SuperUser
                        || user.UserType == UserTypes.Administrator))
                {
                    HydrantWikiManager hwManager = new HydrantWikiManager();

                    string sTagGuid = _parameters["tagGuid"];
                    Guid tagGuid;

                    if (Guid.TryParse(sTagGuid, out tagGuid))
                    {
                        Tag tag = hwManager.GetTag(tagGuid);

                        if (tag != null)
                        {
                            if (tag.Status == TagStatus.Pending)
                            {
                                tag.Status = TagStatus.Rejected;
                                hwManager.Persist(tag);

                                return @"{ ""Result"":""Success"" }";
                            }
                        }

                    }
                }
            }

            return @"{ ""Result"":""Failure"" }";
        }

        private string MatchTagWithHydrant(DynamicDictionary _parameters)
        {
            User user;

            if (AuthHelper.IsAuthorized(Request, out user))
            {
                if (user != null
                    && (user.UserType == UserTypes.SuperUser
                        || user.UserType == UserTypes.Administrator))
                {
                    HydrantWikiManager hwManager = new HydrantWikiManager();

                    string sTagGuid = _parameters["tagGuid"];
                    Guid tagGuid;

                    if (Guid.TryParse(sTagGuid, out tagGuid))
                    {
                        Tag tag = hwManager.GetTag(tagGuid);

                        if (tag != null)
                        {
                            if (tag.Status == TagStatus.Pending)
                            {
                                string sHydrantGuid = _parameters["hydrantGuid"];
                                Guid hydrantGuid;

                                if (Guid.TryParse(sHydrantGuid, out hydrantGuid))
                                {
                                    Hydrant hydrant = hwManager.GetHydrant(hydrantGuid);

                                    if (hydrant != null)
                                    {
                                        tag.Status = TagStatus.Approved;
                                        tag.HydrantGuid = hydrantGuid;
                                        hwManager.Persist(tag);

                                        //TODO - Probably need to reaverage all tag positions and assign back to hydrant
                                        hydrant.LastReviewerUserGuid = user.Guid;
                                        hwManager.Persist(hydrant);

                                        return @"{ ""Result"":""Success"" }";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return @"{ ""Result"":""Failure"" }";
        }

        private string AcceptTag(DynamicDictionary _parameters)
        {
            User user;

            if (AuthHelper.IsAuthorized(Request, out user))
            {
                if (user != null
                    && (user.UserType == UserTypes.SuperUser
                        || user.UserType == UserTypes.Administrator))
                {
                    HydrantWikiManager hwManager = new HydrantWikiManager();

                    string sTagGuid = _parameters["tagGuid"];
                    Guid tagGuid;

                    if (Guid.TryParse(sTagGuid, out tagGuid))
                    {
                        Tag tag = hwManager.GetTag(tagGuid);

                        if (tag != null)
                        {
                            if (tag.Status == TagStatus.Pending)
                            {
                                Hydrant hydrant = new Hydrant
                                {
                                    Active = true,
                                    CreationDateTime = tag.DeviceDateTime,
                                    LastModifiedBy = tag.LastModifiedBy,
                                    LastModifiedDateTime = tag.LastModifiedDateTime,
                                    LastReviewerUserGuid = user.Guid,
                                    OriginalReviewerUserGuid = user.Guid,
                                    OriginalTagDateTime = tag.DeviceDateTime,
                                    OriginalTagUserGuid = tag.UserGuid,
                                    Position = tag.Position,
                                    PrimaryImageGuid = tag.ImageGuid
                                };

                                hwManager.Persist(hydrant);

                                tag.Status = TagStatus.Approved;
                                tag.HydrantGuid = hydrant.Guid;
                                hwManager.Persist(tag);

                                return @"{ ""Result"":""Success"" }";
                            }
                        }
                    }
                }
            }

            return @"{ ""Result"":""Failure"" }";
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

        private string HandleGetPendingTagsInTable(DynamicDictionary _parameters)
        {
            User user;

            if (AuthHelper.IsAuthorized(Request, out user))
            {
                if (user.UserType == UserTypes.SuperUser
                    || user.UserType == UserTypes.Administrator)
                {
                    HydrantWikiManager hwManager = new HydrantWikiManager();
                    List<Tag> tags = hwManager.GetPendingTags();

                    TagTableResponse response = new TagTableResponse {Result = "Success"};

                    foreach (Tag tag in tags)
                    {
                        JsonObjects.ReviewTag jReviewTag = new JsonObjects.ReviewTag(tag);

                        User tagUser = (User)hwManager.GetUser(tag.UserGuid);
                        jReviewTag.Username = tagUser.GetUsernameWithSource();

                        response.Data.Add(jReviewTag);
                    }

                    string json = JsonConvert.SerializeObject(response);

                    return json;
                }
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
                        Position = geoPoint,
                        Status = TagStatus.Pending
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
                        hwManager.Persist(tag);
                        hwManager.LogVerbose(user.Guid, "Tag Saved");

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

                                Image original = ImageHelper.GetImage(data);

                                data = ImageHelper.GetThumbnailBytesOfMaxSize(original, 800);
                                hwManager.PersistWebImage(tag.ImageGuid.Value, ".jpg", "image/jpg", data);
                               
                                data = ImageHelper.GetThumbnailBytesOfMaxSize(original, 100);
                                hwManager.PersistThumbnailImage(tag.ImageGuid.Value, ".jpg", "image/jpg", data);
                            }
                            catch (Exception ex)
                            {
                                hwManager.LogException(user.Guid, ex);
                            }
                        }

                        return @"{ ""Result"":""Success"" }"; 
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