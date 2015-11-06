
using System;

namespace Site.api.Objects
{
    public class Position
    {
        public DateTime DeviceDateTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double Accuracy { get; set; }
        public bool WasAveraged { get; set; }
    }
}