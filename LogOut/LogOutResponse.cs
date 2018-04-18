using Newtonsoft.Json;

namespace WialonHostingSharp
{
    internal sealed class LogOutResponse
    {
        [JsonProperty("error")]
        public LogOutResult Result;
    }
}