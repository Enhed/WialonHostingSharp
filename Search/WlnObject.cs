using Newtonsoft.Json;

namespace WialonHostingSharp.Search
{
    public class WlnObject : Unit
    {
        [JsonProperty("uacl")]
        public string UserAccessLevel;
    }

}