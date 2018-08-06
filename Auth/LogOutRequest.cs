using Newtonsoft.Json;
using WialonHostingSharp.Http;

namespace WialonHostingSharp.Auth
{
    public sealed class LogOutRequest : Request<LogOutResult>
    {
        public LogOutRequest(Session connection) : base(connection, new RequestParams())
        {
        }

        public override string Method => "core/logout";

        protected override LogOutResult Convert(string source)
            => JsonConvert.DeserializeObject<LogOutResponse>(source).Result;
    }
}