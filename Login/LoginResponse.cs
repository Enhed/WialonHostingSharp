using Newtonsoft.Json;

namespace WialonHostingSharp
{
    public sealed class LoginResponse
    {
        [JsonProperty("eid")]
        public string SessionId;
    }
}