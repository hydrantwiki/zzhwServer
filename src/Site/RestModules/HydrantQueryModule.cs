using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HydrantWiki.Library.Helpers;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using Nancy;
using TreeGecko.Library.Geospatial.Objects;

namespace Site.RestModules
{
    public class HydrantQueryModule : NancyModule
    {
        public HydrantQueryModule()
        {
            Get["/rest/hydrants/csv/{east}/{west}/{north}/{south}"] = _parameters =>
            {
                Response response = GetCSVData(_parameters);
                response.ContentType = "text/csv";
                return response;
            };
        }

        public string GetCSVData(DynamicDictionary _parameters)
        {
            HydrantWikiManager hwm = new HydrantWikiManager();
         
            double east = Convert.ToDouble((string)_parameters["east"]);
            double west = Convert.ToDouble((string)_parameters["west"]);
            double north = Convert.ToDouble((string)_parameters["north"]);
            double south = Convert.ToDouble((string)_parameters["south"]);

            GeoBox geobox = new GeoBox(east, west, north, south);

            List<Hydrant> hydrants = hwm.GetHydrants(geobox);

            return HydrantCSVHelper.GetHydrantCSV(hydrants);
        }

    }
}