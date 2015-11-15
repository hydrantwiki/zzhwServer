namespace Site.api.Objects
{
    public class GeoLocation
    {
        public GeoLocation() {  }

        public GeoLocation(double _latitude,
            double _longitude,
            double _altitude)
        {
            Latitude = _latitude;
            Longitude = _longitude;
            Altitude = _altitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }

    }
}