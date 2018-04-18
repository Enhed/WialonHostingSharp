using Newtonsoft.Json;

namespace WialonHostingSharp
{
    public sealed class LoginParams : RequestParams
    {
        [JsonProperty("token")]
        public string Token;

        [JsonProperty("operateAs", NullValueHandling = NullValueHandling.Ignore)]
        public string User;

        [JsonProperty("fl")]
        public LoginFlags Flag = LoginFlags.Base;
    }
}