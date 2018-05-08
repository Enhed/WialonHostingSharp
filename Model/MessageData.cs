using System.Collections.Generic;
using Newtonsoft.Json;

namespace WialonHostingSharp
{
    public sealed class MessageData : Message
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