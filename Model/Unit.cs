using Newtonsoft.Json;

namespace WialonHostingSharp
{
    public class Unit
    {
        [JsonProperty("nm")]
        public string Name;

        [JsonProperty("id")]
        public uint Id;
    }

}