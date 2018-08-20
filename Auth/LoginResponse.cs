using Newtonsoft.Json;
using WialonHostingSharp.Http;

namespace WialonHostingSharp.Auth
{
    public sealed class LoginResponse
    {
        [JsonProperty("eid")]
        public string SessionId;

        [JsonProperty("token")]
        public Token Token;
    }
}