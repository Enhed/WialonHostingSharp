using System;
using System.Threading.Tasks;
using WialonHostingSharp.Search;

namespace WialonHostingSharp
{
    public class Session : Connection, IDisposable
    {
        public Session(string host, string id) : base(host)
        {
            Id = id;
        }

        private const string SID_NAME = "sid";
        public readonly string Id;

        public override string GenerateUrl( string method, string json )
        {
            var m = ( METHOD_NAME, method );
            var p = ( PRMS_NAME, json );
            var s = ( SID_NAME, Id );

            return GenerateUrl( m, s, p );
        }

        public Task Logout()
        {
            return SessionManager.Logout(this);
        }

        public void Dispose()
        {
            this.Logout().Wait();
        }

        public SearchService CreateSearchService() => new SearchService(this);
    }
}