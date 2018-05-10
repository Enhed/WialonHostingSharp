using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WialonHostingSharp.Messages
{
    public class Message
    {
        [JsonConverter(typeof(UnixDateTimeConverter))]
        [JsonProperty("t")]
        public DateTime UtcTime;

        [JsonIgnore]
        public DateTime LocalTime => UtcTime.ToLocalTime();

        [JsonProperty("f")]
        public uint Flags;

        [JsonProperty("tp")]
        public string Type;
    }
}