using Newtonsoft.Json;

namespace WialonHostingSharp
{
    public sealed class LogOutRequest : Request<LogOutResult>
    {
        public LogOutRequest(Session connection) : base(connection, RequestParams.Empty)
        {
        }

        public override string Method => "core/logout";

        protected override LogOutResult Convert(string source)
            => JsonConvert.DeserializeObject<LogOutResponse>(source).Result;
    }
}