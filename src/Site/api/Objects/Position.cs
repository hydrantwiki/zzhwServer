
using System;

namespace Site.api.Objects
{
    public class Position : GeoLocation
    {
        public DateTime DeviceDateTime { get; set; }
        public double Accuracy { get; set; }
        public bool WasAveraged { get; set; }
    }
}