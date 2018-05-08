using Newtonsoft.Json;
using WialonHostingSharp;

namespace WialonHostingSharp.Messages
{
    public sealed class LoadMessageResult<T>
        where T : Message
    {
        [JsonProperty("count")]
        public uint Count;

        [JsonProperty("messages")]
        public T[] Messages;
    }
}
