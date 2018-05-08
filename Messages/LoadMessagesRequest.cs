using WialonHostingSharp;
using WialonHostingSharp.Http;

namespace WialonHostingSharp.Messages
{
    public sealed class LoadMessagesRequest<T> : Request<LoadMessageResult<T>>
        where T : Message
    {
        public LoadMessagesRequest(Connection connection, RequestParams parameters)
            : base(connection, parameters)
        {
        }

        public override string Method => "messages/load_interval";
    }
}
