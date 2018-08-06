using System.Linq;

namespace WialonHostingSharp
{
    public class Connection
    {
        public Connection(string host)
        {
            Host = host;
        }

        protected const string REL_PATH = "wialon/ajax.html";
        protected const string METHOD_NAME = "svc";
        protected const string PRMS_NAME = "params";
        private const string HTTP = "http";

        public bool Ssl;
        public string Protocol => !Ssl ? HTTP : $"{HTTP}s";
        public readonly string Host;

        public string GenerateUrl( params ( string name, string value )[] parameters )
        {
            var ar = parameters.Select(x => $"{x.name}={x.value}");
            var prms = string.Join("&", ar);

            return $"{Protocol}://{Host}/{REL_PATH}?{prms}";
        }

        public virtual string GenerateUrl( string method, string json )
        {
            var m = ( METHOD_NAME, method );
            var p = ( PRMS_NAME, json );

            return GenerateUrl( m, p );
        }

        public virtual string GenerateUrl( string method )
        {
            var m = ( METHOD_NAME, method );

            return GenerateUrl(m);
        }
    }
}