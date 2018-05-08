using Newtonsoft.Json;

namespace WialonHostingSharp.Search
{
    public class Unit
    {
        [JsonProperty("nm")]
        public string Name;

        [JsonProperty("id")]
        public uint Id;
    }

}