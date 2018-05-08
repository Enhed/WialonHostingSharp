using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WialonHostingSharp.Http;

namespace WialonHostingSharp.Messages
{
    public sealed class LoadMessagesParams : RequestParams
    {
        [JsonProperty("itemId")]
        public long ItemId;

        [JsonProperty("timeFrom")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime BeginDate;

        [JsonProperty("timeTo")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime EndDate;

        [JsonProperty("flags")]
        public long Flags;

        [JsonProperty("flagsMask")]
        public long Mask;

        [JsonProperty("loadCount")]
        public long Count = 0xffffffff;
    }
}
