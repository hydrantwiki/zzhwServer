using System;
using System.Drawing;
using System.Linq;
using HydrantWiki.Library.Constants;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using Nancy;
using Site.Extensions;
using Site.Helpers;
using TreeGecko.Library.Common.Helpers;
using TreeGecko.Library.Geospatial.Objects;

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

            Post["/api/tag"] = _parameters =>
            {
                Response response = (Response) HandleTagPost(_parameters);
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

        private string HandleTagPost(DynamicDictionary _parameters)
        {
            User user;

            if (AuthHelper.IsAuthorized(Request, out user))
            {
                HydrantWikiManager hwManager = new HydrantWikiManager();

                string json = Request.Body.ReadAsString();




            //    string sLongitude = Request.Form["longitudeInput"];
            //    string sAccuracy = Request.Form["accuracyInput"];
            //    string sDeviceDateTime = Request.Form["positionDateTimeInput"];

            //    double lat = 0.0;
            //    double lon = 0.0;
            //    double accuracy = -1;

            //    DateTime deviceDateTime = DateTime.MinValue;
            //    GeoPoint geoPoint = null;

            //    if (Double.TryParse(sLatitude, out lat))
            //    {
            //        if (Double.TryParse(sLongitude, out lon))
            //        {
            //            //Ignore positions that are 0.0 and 0.0 exactly
            //            if (!(lat.Equals(0)
            //                  && lon.Equals(0)))
            //            {
            //                geoPoint = new GeoPoint { X = lon, Y = lat };
            //            }
            //        }
            //    }

            //    Double.TryParse(sAccuracy, out accuracy);
            //    DateTime.TryParse(sDeviceDateTime, out deviceDateTime);

            //    //If we got a timestamp that was a zero date, ignore it and use now.
            //    if (deviceDateTime == DateTime.MinValue)
            //    {
            //        deviceDateTime = DateTime.UtcNow;
            //    }

            //    //We will accept a tag without a photo, but not one without a position.
            //    if (geoPoint != null)
            //    {
            //        Tag tag = new Tag
            //        {
            //            Active = true,
            //            DeviceDateTime = deviceDateTime,
            //            LastModifiedDateTime = DateTime.UtcNow,
            //            UserGuid = user.Guid,
            //            VersionTimeStamp = DateTime.UtcNow.ToString("u"),
            //            Position = geoPoint,
            //            Status = TagStatus.Pending
            //        };

            //        if (Request.Files.Any())
            //        {
            //            tag.ImageGuid = Guid.NewGuid();
            //        }
            //        else
            //        {
            //            tag.ImageGuid = null;
            //        }

            //        try
            //        {
            //            hwManager.Persist(tag);
            //            hwManager.LogVerbose(user.Guid, "Tag Saved");

            //            if (tag.ImageGuid != null)
            //            {
            //                HttpFile file = Request.Files.First();

            //                long fileSize = file.Value.Length;

            //                try
            //                {
            //                    byte[] data = new byte[fileSize];

            //                    file.Value.Read(data, 0, (int)fileSize);

            //                    hwManager.PersistOriginal(tag.ImageGuid.Value, ".jpg", "image/jpg", data);
            //                    hwManager.LogVerbose(user.Guid, "Tag Image Saved");

            //                    Image original = ImageHelper.GetImage(data);

            //                    data = ImageHelper.GetThumbnailBytesOfMaxSize(original, 800);
            //                    hwManager.PersistWebImage(tag.ImageGuid.Value, ".jpg", "image/jpg", data);

            //                    data = ImageHelper.GetThumbnailBytesOfMaxSize(original, 100);
            //                    hwManager.PersistThumbnailImage(tag.ImageGuid.Value, ".jpg", "image/jpg", data);
            //                }
            //                catch (Exception ex)
            //                {
            //                    hwManager.LogException(user.Guid, ex);
            //                }
            //            }

            //            return @"{ ""Result"":""Success"" }";
            //        }
            //        catch (Exception ex)
            //        {
            //            hwManager.LogException(user.Guid, ex);
            //        }
            //    }
            //    else
            //    {
            //        //No position
            //        hwManager.LogWarning(user.Guid, "No position");

            //        return @"{ ""Result"":""Failure - No position"" }";
            //    }
            }


            return @"{ ""Result"":""Failure"" }";
        }

    }
}