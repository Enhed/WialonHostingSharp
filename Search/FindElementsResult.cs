using Newtonsoft.Json;

namespace WialonHostingSharp
{
    public class FindElementsResult<T>
    {
        [JsonProperty("totalItemsCount")]
        public int Count;

        [JsonProperty("items")]
        public T[] Items;
    }

}