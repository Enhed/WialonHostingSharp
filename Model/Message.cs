using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WialonHostingSharp
{
    public class Message
    {
        [JsonConverter(typeof(UnixDateTimeConverter))]
        [JsonProperty("t")]
        public DateTime Time;

        [JsonProperty("f")]
        public uint Flags;

        [JsonProperty("tp")]
        public string Type;
    }
}