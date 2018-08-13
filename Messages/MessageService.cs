using System.Threading.Tasks;
using System;
using WialonHostingSharp;
using WialonHostingSharp.Http;
using Newtonsoft.Json;
using SharpExtension.For;
using System.Linq;
using SharpExtension;

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

        public Task<(bool Success, string Response)> Delete(uint index)
        {
            return new DeleteMessageRequest
            (
                session,
                new DeleteMessageRequest.Params{ MessageIndex = index }
            ).GetResponse();
        }

        public async Task Delete(Message[] messages, int maxIter = 10 )
        {
            var count = messages.Length / maxIter;
            var index = 0;
            
            for(var i = 0; i <= count; ++i)
            {
                var iend = index + maxIter;

                var asd = new uint[10];
                
                var tasks = (index, Math.Min(messages.Length, iend)).ForGet(x => x).Select( x => Delete((uint)x) );
                var resps = await Task.WhenAll(tasks);

                var fullsuccess = resps.All(x => x.Success)
#if !DEBUG
                    ;
#else
                    .Do(x => Console.WriteLine($"Success delete {index} - {iend}"));
#endif

                if(!fullsuccess)
                    throw new Exception( $"success x[{resps.Count(x => x.Success)}] {resps.First(x => !x.Success).Response}" );

                index = iend;
            }
        }

        public async Task Delete(int id, DateTime begin, DateTime end, int maxIter = 10)
        {
            var messages = await GetDataMessages(id, begin, end);
            await Delete(messages, maxIter);
        }
    }

    public sealed class DeleteMessageRequest : Request<(bool Success, string Response)>
    {
        public const string SuccessResponse = "{}";

        public DeleteMessageRequest(Session connection, Params parameters)
            : base(connection, parameters)
        {
        }

        public override string Method => "messages/delete_message";

        public sealed class Params : RequestParams
        {
            [JsonProperty("msgIndex")]
            public uint MessageIndex;
        }

        protected override (bool Success, string Response) Convert(string source)
        {
            return ( source.Trim() == SuccessResponse, source );
        }
    }
}