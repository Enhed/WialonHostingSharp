using Newtonsoft.Json;

namespace WialonHostingSharp.Messages
{
    public sealed class Position
    {
        [JsonProperty("x")]
        public double Latitude;

        [JsonProperty("y")]
        public double Longitude;

        [JsonProperty("z")]
        public double Altitude;

        [JsonProperty("s")]
        public uint Speed;

        [JsonProperty("c")]
        public uint Course;

        [JsonIgnore]
        public bool IsValid => Latitude > 0 && Longitude > 0;

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