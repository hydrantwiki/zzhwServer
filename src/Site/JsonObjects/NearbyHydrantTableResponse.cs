using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HydrantWiki.Library.Objects;

namespace Site.JsonObjects
{
    public class NearbyHydrantTableResponse
    {
        public string Result { get; set; }

        public NearbyHydrantTableResponse()
        {
            Data = new List<NearbyHydrant>();
        }

        public List<NearbyHydrant> Data { get; set; } 
    }
}