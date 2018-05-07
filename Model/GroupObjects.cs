using System.Threading.Tasks;
using Newtonsoft.Json;
using WialonHostingSharp;

namespace WialonHostingSharp
{
    public sealed class GroupObjects : WlnObject
    {
        [JsonProperty("u")]
        public long[] ObjectIds;

        public Task<WlnObject[]> GetObjects(SearchService ss)
        {
            if(ObjectIds?.Length == null) return null;
            return ss.GetObjects(string.Join(",",ObjectIds), PropertyElement.sys_id.ToString());
        }
    }

}