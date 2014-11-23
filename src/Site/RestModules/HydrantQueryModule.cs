using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HydrantWiki.Library.Helpers;
using HydrantWiki.Library.Managers;
using HydrantWiki.Library.Objects;
using Nancy;
using Newtonsoft.Json;
using Site.JsonObjects;
using TreeGecko.Library.Geospatial.Objects;
using Tag = HydrantWiki.Library.Objects.Tag;

namespace Site.RestModules
{
    public class HydrantQueryModule : NancyModule
    {
        public HydrantQueryModule()
        {
            Get["/rest/hydrants/csv/{east}/{west}/{north}/{south}"] = _parameters =>
            {
                Response response = GetGeoboxCSVData(_parameters);
                response.ContentType = "text/csv";
                return response;
            };

            Get["/rest/nearbyhydrants/csv/{latitude}/{longitude}/{distance}"] = _parameters =>
            {
                Response response = GetCenterRadiusCSVData(_parameters);
                response.ContentType = "text/csv";
                return response;
            };

            Get["/rest/nearbyhydrants/table/{latitude}/{longitude}/{distance}"] = _parameters =>
            {
                Response response = GetCenterRadiusCSVData(_parameters);
                response.ContentType = "application/json";
                return response;
            };
        }

        public string GetCenterRadiusJsonData(DynamicDictionary _parameters)
        {
            HydrantWikiManager hwm = new HydrantWikiManager();

            double latitude = Convert.ToDouble((string)_parameters["latitude"]);
            double longitude = Convert.ToDouble((string)_parameters["longitude"]);
            double distance = Convert.ToDouble((string)_parameters["distance"]);

            GeoPoint point = new GeoPoint(longitude, latitude);

            List<NearbyHydrant> hydrants = hwm.GetNearbyHydrants(point, distance);
            NearbyHydrantTableResponse response = new NearbyHydrantTableResponse { Result = "Success" };
            response.Data = hydrants;
            
            string json = JsonConvert.SerializeObject(response);

            return json;
        }

        public string GetCenterRadiusCSVData(DynamicDictionary _parameters)
        {
            HydrantWikiManager hwm = new HydrantWikiManager();

            double latitude = Convert.ToDouble((string)_parameters["latitude"]);
            double longitude = Convert.ToDouble((string)_parameters["longitude"]);
            double distance = Convert.ToDouble((string)_parameters["distance"]);

            GeoPoint point = new GeoPoint(longitude, latitude);

            List<NearbyHydrant> hydrants = hwm.GetNearbyHydrants(point, distance);
            return HydrantCSVHelper.GetHydrantCSV(hydrants);
        }

        public string GetGeoboxCSVData(DynamicDictionary _parameters)
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