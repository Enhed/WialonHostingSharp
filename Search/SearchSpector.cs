using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WialonHostingSharp.Search;

namespace WialonHostingSharp.Search
{
    public sealed class SearchSpector
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("itemsType")]
        public ItemType ItemType = ItemType.Object;

        [JsonProperty("propName")]
        public string PropertyName = PropertyElement.sys_name.ToString();

        [JsonProperty("sortType")]
        public string SortType = PropertyElement.sys_name.ToString();

        [JsonProperty("propValueMask")]
        public string Mask = "*";

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("propType", NullValueHandling = NullValueHandling.Ignore)]
        public PropertyType? PropertyType;
    }

}