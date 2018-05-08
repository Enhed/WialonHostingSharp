using Newtonsoft.Json;

namespace WialonHostingSharp.Auth
{
    public sealed class LoginResponse
    {
        [JsonProperty("eid")]
        public string SessionId;
    }
}