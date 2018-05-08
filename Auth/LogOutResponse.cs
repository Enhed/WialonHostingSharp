using Newtonsoft.Json;

namespace WialonHostingSharp.Auth
{
    internal sealed class LogOutResponse
    {
        [JsonProperty("error")]
        public LogOutResult Result;
    }
}