using System.Collections.Generic;
using Newtonsoft.Json;

namespace WialonHostingSharp.Messages
{
    public sealed class DataMessage : Message
    {
        [JsonProperty("pos")]
        public Position Position;

        [JsonProperty("i")]
        public uint DataIn;

        [JsonProperty("o")]
        public uint DataOut;

        [JsonProperty("p")]
        public Dictionary<string, double> Parameters;
    }
}