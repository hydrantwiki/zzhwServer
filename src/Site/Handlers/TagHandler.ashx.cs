using System;
using System.Web;
using System.Web.SessionState;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using TreeGecko.Library.Geospatial.Objects;
using TreeGecko.Library.Net.Handlers;
using TreeGecko.Library.Net.Helpers;

namespace HydrantWiki.Web.Handlers
{
    /// <summary>
    /// Summary description for TagHandler
    /// </summary>
    public class TagHandler : AbstractHandler, IRequiresSessionState
    {
        public override void HandleGet(HttpContext _context)
        {
            HydrantWikiManager hwManager = new HydrantWikiManager();

            if (LoginHelper.LoginAndCreateSession(hwManager, _context))
            {
                User user = (User)_context.Session["User"];

                if (user != null)
                {
                    string sLatitude = _context.Request.Form["latitudeInput"];
                    string sLongitude = _context.Request.Form["longitudeInput"];
                    string sAccuracy = _context.Request.Form["accuracyInput"];
                    string sDeviceDateTime = _context.Request.Form["positionDateTimeInput"];
                    
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
                                geoPoint = new GeoPoint { X = lon, Y = lat };
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

                        if (_context.Request.Files.Count > 0)
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
                                HttpPostedFile file = _context.Request.Files[0];

                                int fileSize = file.ContentLength;

                                try
                                {
                                    byte[] data = new byte[fileSize];
                                    file.InputStream.Read(data, 0, fileSize);

                                    hwManager.PersistOriginal(tag.ImageGuid.Value, ".jpg", "image/jpg", data);

                                    hwManager.LogVerbose(user.Guid, "Tag Image Saved");
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


                        hwManager.LogVerbose(user.Guid, "Tag Enqueued");
                    }
                    else
                    {
                        //No position
                        hwManager.LogWarning(user.Guid, "No position");
                    }
                }
                else
                {
                    //Users don't match
                    hwManager.LogWarning(Guid.Empty, "User not found");
                }
            }
            else
            {
                //Not logged in
                hwManager.LogWarning(Guid.Empty, "User not logged in");
            }
        }
    }
}