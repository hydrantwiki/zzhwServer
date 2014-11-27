using System;
using System.Collections.Generic;
using System.Text;
using HydrantWiki.Library.Helpers;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using Nancy;
using TreeGecko.Library.Common.Helpers;
using TreeGecko.Library.Common.Objects;

namespace Site.RestModules
{
    public class HydrantDetailModule : NancyModule
    {
        public HydrantDetailModule()
        {
            Get["/rest/hydrant/{hydrantguid}"] = _parameters =>
            {
                Context.ViewBag.MapBoxMap = Config.GetSettingValue("MapBoxMap");
                Context.ViewBag.MapBoxKey = Config.GetSettingValue("MapBoxKey");

                Response response = (Response)HandleGet(_parameters);
                response.ContentType = "application/json";
                return response;
            };
        }

        private string HandleGet(DynamicDictionary _parameters)
        {
            HydrantWikiManager hwManager = new HydrantWikiManager();

            StringBuilder sb = new StringBuilder();

            string sGuid = _parameters["hydrantguid"];

            if (sGuid != null)
            {
                Guid guid = new Guid(sGuid);

                Hydrant hydrant = hwManager.GetHydrant(guid);

                if (hydrant != null)
                {
                    sb.Append("<table width=\"100%\" >");
                    sb.Append("<tr class=\"hdHeaderRow\">");
                    sb.Append("<td class=\"hdHeaderColumn\" colspan=\"2\"><h3>Hydrant</h3></td>");
                    sb.Append("</tr>");

                    if (hydrant.Position != null)
                    {
                        sb.Append("<tr class=\"hdPosition\" >");
                        sb.Append("<td class=\"hdPositionTitle\"><strong>Latitude</strong></td>");
                        sb.AppendFormat("<td class=\"hdPositionValue\">{0}</td>",
                            hydrant.Position.Y.ToString("0.00000000"));
                        sb.Append("</tr>");
                        sb.Append("<tr class=\"hdPosition\">");
                        sb.Append("<td class=\"hdPositionTitle\"><strong>Longitude</strong></td>");
                        sb.AppendFormat("<td class=\"hdPositionValue\">{0}</td>",
                            hydrant.Position.X.ToString("0.00000000"));
                        sb.Append("</tr>");
                    }

                    //Add the primary image
                    string thumbNailURL = hydrant.GetUrl(true);
                    string mediumSizeURL = hydrant.GetUrl(false);

                    if (thumbNailURL != null
                        && mediumSizeURL != null)
                    {
                        sb.Append("<tr class=\"hdImageRow\" >");
                        sb.Append("<td class=\"hdImageColumn\" colspan=\"2\">");
                        sb.Append("<a href=\"");
                        sb.Append(mediumSizeURL);
                        sb.Append("\" target=\"_blank\" >");
                        sb.Append("<img src=\"");
                        sb.Append(thumbNailURL);
                        sb.Append("\" alt=\"Image\"/></a></td>");
                        sb.Append("</tr>");
                    }

                    if (hydrant.Properties.Count > 0)
                    {
                        //Add the tagger attribution
                        sb.Append("<tr class=\"hdTaggersRow\"  >");
                        sb.Append("<td class=\"hdTaggersColumn\" colspan=\"2\"><h4>Extended Properties</h4></td>");


                        foreach (Property prop in hydrant.Properties)
                        {
                            sb.Append("<tr class=\"hdPosition\" >");
                            sb.Append("<td class=\"hdPositionTitle\"><strong>");
                            sb.Append(prop.Name);
                            sb.Append("</strong></td>");
                            sb.AppendFormat("<td class=\"hdPositionValue\">{0}</td>", prop.Value);
                            sb.Append("</tr>");
                        }
                    }

                    //Add the tagger attribution
                    sb.Append("<tr class=\"hdTaggersRow\" >");
                    sb.Append("<td class=\"hdTaggersColumn\" colspan=\"2\"><h4>Taggers</h4></td>");

                    Dictionary<Guid, User> users = new Dictionary<Guid, User>();

                    List<Tag> tags = hwManager.GetTags(hydrant.Guid);
                    foreach (Tag tag in tags)
                    {
                        if (!users.ContainsKey(tag.UserGuid))
                        {
                            User user = (User) hwManager.GetUser(tag.UserGuid);

                            users.Add(user.Guid, user);
                        }
                    }

                    foreach (User user in users.Values)
                    {
                        string source;
                        string userName;

                        if (user.UserSource.Equals("OSM", StringComparison.InvariantCultureIgnoreCase))
                        {
                            source = "Open Street Maps";
                            userName = user.DisplayName;
                        }
                        else if (user.UserSource.Equals("Yo", StringComparison.InvariantCultureIgnoreCase))
                        {
                            source = "Yo";
                            userName = user.DisplayName;
                        }
                        else
                        {
                            source = "HydrantWiki";
                            userName = user.DisplayName;
                        }

                        sb.Append("<tr class=\"hdTaggerRow\"  >");
                        sb.AppendFormat("<td class=\"hdTaggerColumn\" colspan=\"2\" >{0} : {1}</td>", 
                            source,
                            userName);
                        sb.Append("</tr>");
                    }

                    sb.Append("</table>");

                    return sb.ToString();
                }                               
            }
            
            return "<strong>Hydrant Not Found</strong>";
        }
    }
}