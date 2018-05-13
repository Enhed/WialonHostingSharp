using Newtonsoft.Json;

namespace WialonHostingSharp.Search
{
    public class Unit
    {
        [JsonProperty("nm", NullValueHandling = NullValueHandling.Ignore)]
        public string Name;

        [JsonProperty("id")]
        public uint Id;
    }

}