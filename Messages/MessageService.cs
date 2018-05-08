using System.Threading.Tasks;
using System;
using WialonHostingSharp;

namespace WialonHostingSharp.Messages
{
    public sealed class MessageService
    {
        public readonly Session session;

        public MessageService(Session session)
        {
            this.session = session;
        }

        public async Task<DataMessage[]> GetDataMessages(int id, DateTime begin, DateTime end,
            long count = 0xffffffff, long flags = 0)
        {
            var param = new LoadMessagesParams
            {
                ItemId = id,
                Flags = flags,
                BeginDate = begin,
                EndDate = end,
                Count = count
            };

            var req = new LoadMessagesRequest<DataMessage>(session, param);
            var result = await req.GetResponse();
            return result.Messages;
        }
    }
}