using System.Threading.Tasks;
using WialonHostingSharp.Search;

namespace WialonHostingSharp.Extensions
{
    public static class GroupObjectsExtension
    {
        public static Task<WlnObject[]> GetObjects(this GroupObjects obj, SearchService ss)
        {
            if(obj.ObjectIds?.Length == null) return null;
            return ss.GetObjects(string.Join(",",obj.ObjectIds), PropertyElement.sys_id.ToString());
        }
    }
}