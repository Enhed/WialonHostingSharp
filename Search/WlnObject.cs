using Newtonsoft.Json;

namespace WialonHostingSharp.Search
{
    public class WlnObject : Unit
    {
        [JsonProperty("uacl")]
        public string UserAccessLevel;

        [JsonProperty("uid")]
        public string Uid;

        [JsonProperty("uid2")]
        public string Uid2;

        [JsonProperty("hw")]
        public long? HardwareType;

        [JsonProperty("ph")]
        public string Phone;

        [JsonProperty("ph2")]
        public string Phone2;

        [JsonProperty("psw")]
        public string Password;
    }

}