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

        public Task<DataMessage[]> GetDataMessages(int id, DateTime begin, DateTime end,
            long count = 0xffffffff, long flags = 0, long mask = 0)
        {
            return GetMessages<DataMessage>(id, begin, end, count, flags, mask);
        }

        public async Task<T[]> GetMessages<T>(int id, DateTime begin, DateTime end,
            long count = 0xffffffff, long flags = 0, long mask = 0)
            where T : Message
        {
            var param = new LoadMessagesParams
            {
                ItemId = id,
                Flags = flags,
                BeginDate = begin,
                EndDate = end,
                Count = count,
                Mask = mask
            };

            var req = new LoadMessagesRequest<T>(session, param);
            var result = await req.GetResponse();
            return result.Messages;
        }
    }
}