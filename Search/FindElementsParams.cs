using Newtonsoft.Json;
using WialonHostingSharp.Http;
using WialonHostingSharp.Search;

namespace WialonHostingSharp.Search
{
    public class FindElementsParams : RequestParams
    {


        [JsonProperty("spec")]
        public SearchSpector Spector = new SearchSpector();

        [JsonProperty("force")]
        public uint Force = 1;

        [JsonProperty("flags")]
        public long Flags = 1;

        [JsonProperty("from")]
        public uint From = 0;

        [JsonProperty("to")]
        public uint To = 100;
    }

}