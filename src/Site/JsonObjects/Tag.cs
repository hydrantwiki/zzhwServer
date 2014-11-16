using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HydrantWiki.Library.Helpers;
using TreeGecko.Library.Common.Helpers;
using TreeGecko.Library.Geospatial.Geoframeworks.Objects;

namespace Site.JsonObjects
{
    public class Tag
    {
        public string Location { get; set; }
        public string TagDateTime { get; set; }
        public string Thumbnail { get; set; }

        public Tag(HydrantWiki.Library.Objects.Tag _tag)
        {
            TagDateTime = _tag.DeviceDateTime.ToString("G");

            Thumbnail = string.Format("<img src=\"{0}\" class=\"tagimg\">", _tag.GetUrl(true));

            if (_tag.Position != null)
            {
                Location = string.Format("Latitude - {0}<br>Longitude - {1}<br><button type=\"button\" class=\"btn btn-info\" onclick=\"TagMap('{2}')\">View</button>",
                    _tag.Position.X.ToString("###.######"), 
                    _tag.Position.X.ToString("###.######"),
                    _tag.Guid);
            }
        }

    }
}