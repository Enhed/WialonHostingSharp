using Newtonsoft.Json;

namespace WialonHostingSharp.Search
{
    public class FindElementsResult<T>
    {
        [JsonProperty("totalItemsCount")]
        public int Count;

        [JsonProperty("items")]
        public T[] Items;
    }

}