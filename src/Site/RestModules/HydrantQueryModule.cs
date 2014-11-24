using System;
using System.Collections.Generic;
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


            Get["/rest/nearbyhydrantsbytag/csv/{guid}"] = _parameters =>
            {
                string sTagGuid = _parameters["guid"];
                Guid tagGuid;

                if (Guid.TryParse(sTagGuid, out tagGuid))
                {
                    HydrantWikiManager hwm = new HydrantWikiManager();
                    Tag tag = hwm.GetTag(tagGuid);

                    if (tag != null
                        && tag.Position != null)
                    {
                        Response response = GetCenterRadiusCSVData(tag.Position.Y, tag.Position.X, 200, true);
                        response.ContentType = "text/csv";
                        return response;
                    }
                }

                return null;
            };

            Get["/rest/nearbyhydrantsbytag/table/{guid}"] = _parameters =>
            {
                string sTagGuid = _parameters["guid"];
                Guid tagGuid;

                if (Guid.TryParse(sTagGuid, out tagGuid))
                {
                    HydrantWikiManager hwm = new HydrantWikiManager();
                    Tag tag = hwm.GetTag(tagGuid);

                    if (tag != null
                        && tag.Position != null)
                    {
                        Response response = GetCenterRadiusJsonData(tag.Position.Y, tag.Position.X, 200);
                        response.ContentType = "application/json";
                        return response;
                    }
                }

                return null;
            };
        }

        public string GetCenterRadiusJsonData(DynamicDictionary _parameters)
        {
            
            double latitude = Convert.ToDouble((string)_parameters["latitude"]);
            double longitude = Convert.ToDouble((string)_parameters["longitude"]);
            double distance = Convert.ToDouble((string)_parameters["distance"]);

            return GetCenterRadiusJsonData(latitude, longitude, distance);
        }

        public string GetCenterRadiusJsonData(double _latitude, double _longitude, double _distance)
        {
            HydrantWikiManager hwm = new HydrantWikiManager();

            GeoPoint point = new GeoPoint(_longitude, _latitude);

            List<NearbyHydrant> hydrants = hwm.GetNearbyHydrants(point, _distance);
            NearbyHydrantTableResponse response = new NearbyHydrantTableResponse
            {
                Result = "Success",
                Data = hydrants
            };

            string json = JsonConvert.SerializeObject(response);
            return json;
        }

        public string GetCenterRadiusCSVData(DynamicDictionary _parameters)
        {
            double latitude = Convert.ToDouble((string)_parameters["latitude"]);
            double longitude = Convert.ToDouble((string)_parameters["longitude"]);
            double distance = Convert.ToDouble((string)_parameters["distance"]);

            return GetCenterRadiusCSVData(latitude, longitude, distance);
        }

        public string GetCenterRadiusCSVData(double _latitude, double _longitude, double _distance,
            bool _includeCenter = false)
        {
            HydrantWikiManager hwm = new HydrantWikiManager();

            GeoPoint point = new GeoPoint(_longitude, _latitude);

            List<NearbyHydrant> hydrants = hwm.GetNearbyHydrants(point, _distance);
            if (_includeCenter)
            {
                NearbyHydrant center = new NearbyHydrant()
                {
                    Color = "#F51D5A",
                    Symbol = "cross",
                    DistanceInFeet = "0.0",
                    Position = new GeoPoint(_longitude, _latitude),
                    HydrantGuid = Guid.Empty,
                    Title = "Tag"
                };

                hydrants.Add(center);
            }
            string csv = HydrantCSVHelper.GetHydrantCSV(hydrants);
            return csv;
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