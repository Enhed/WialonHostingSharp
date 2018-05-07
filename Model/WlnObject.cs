using Newtonsoft.Json;

namespace WialonHostingSharp
{
    public class WlnObject : Unit
    {
        [JsonProperty("uacl")]
        public uint? UserAccessLevel;
    }

}