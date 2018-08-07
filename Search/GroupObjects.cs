using System.Threading.Tasks;
using Newtonsoft.Json;
using WialonHostingSharp;

namespace WialonHostingSharp.Search
{
    public sealed class GroupObjects : Unit
    {
        [JsonProperty("u")]
        public long[] ObjectIds;
    }
}