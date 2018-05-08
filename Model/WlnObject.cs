using Newtonsoft.Json;

namespace WialonHostingSharp
{
    public class WlnObject : Unit
    {
        [JsonProperty("uacl")]
        public string UserAccessLevel;
    }

}