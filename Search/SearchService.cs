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

        public async Task<T[]> GetUnits<T>(SearchSpector searchSpector, uint force, long flags,
            uint from, uint to)
            where T : Unit
        {
            var param = new FindElementsParams
            {
                Spector = searchSpector,
                Force = force,
                Flags = flags,
                From = from,
                To = to
            };

            var req = new FindElementsRequest<T>(session, param);
            var result = await req.GetResponse();
            return result.Items;
        }

        public Task<WlnObject[]> GetObjects(string mask = "*",
            PropertyElement propName = PropertyElement.sys_name, long flags = 1,
            uint force = 1, uint from = 0, uint to = 0)
        {
            var ss = new SearchSpector
            {
                ItemType = ItemType.Object,
                PropertyName = propName.ToString(),
                Mask = mask
            };

            return GetUnits<WlnObject>(ss, force, flags, from, to);
        }

        public Task<GroupObjects[]> GetGroupObjects(string mask = "*",
            PropertyElement propName = PropertyElement.sys_name, long flags = 1,
            uint force = 1, uint from = 0, uint to = 0)
        {
            var ss = new SearchSpector
            {
                ItemType = ItemType.GroupObjects,
                PropertyName = propName.ToString(),
                Mask = mask
            };

            return GetUnits<GroupObjects>(ss, force, flags, from, to);
        }
    }

}