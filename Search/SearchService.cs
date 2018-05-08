using System.Threading.Tasks;
using WialonHostingSharp.Search;

namespace WialonHostingSharp.Search
{
    public sealed class SearchService
    {
        private readonly Session session;

        public SearchService(Session session)
        {
            this.session = session;
        }

        public async Task<WlnObject[]> GetObjects(string mask = "*", string propName = "sys_name", long flags = 1)
        {
            var ss = new SearchSpector
            {
                PropertyName = propName,
                Mask = mask
            };

            var fer = new FindElementsParams
            {
                Spector = ss,
                Flags = flags
            };

            var req = new FindElementsRequest<WlnObject>(session, fer);
            var resp = await req.GetResponse();
            return resp.Items;
        }

        public async Task<GroupObjects[]> GetGroupObjects(string mask = "*", string propName = "sys_name", long flags = 1)
        {
            var ss = new SearchSpector
            {
                ItemType = ItemType.avl_unit_group,
                PropertyName = propName,
                Mask = mask
            };

            var fer = new FindElementsParams
            {
                Spector = ss,
                Flags = flags
            };

            var req = new FindElementsRequest<GroupObjects>(session, fer);
            var resp = await req.GetResponse();
            return resp.Items;
        }
    }

}