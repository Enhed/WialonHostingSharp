using Newtonsoft.Json;

namespace WialonHostingSharp
{
    public sealed class Position
    {
        [JsonProperty("x")]
        public double Latitude;

        [JsonProperty("y")]
        public double Longitude;

        [JsonProperty("z")]
        public int Altitude;

        [JsonProperty("s")]
        public uint Speed;

        [JsonProperty("c")]
        public uint Course;

        [JsonProperty("sc")]
        public byte SatelliteCount;

        public override string ToString()
        {
            var lat = $"lat:{Latitude}";
            var lon = $"lon:{Longitude}";
            var alt = $"alt:{Altitude}";
            var spd = $"spd:{Speed}";
            var crs = $"crs:{Course}";
            var sats = $"sats: {SatelliteCount}";

            return $"{lat}, {lon}, {alt}, {spd}, {crs}, {sats}";
        }
    }
}