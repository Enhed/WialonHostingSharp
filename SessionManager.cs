using System.Threading.Tasks;
using WialonHostingSharp.Auth;

namespace WialonHostingSharp
{
    public static class SessionManager
    {
        public static async Task<Session> Login(string host, string token, bool ssl = false)
        {
            var connection = new Connection(host);
            connection.Ssl = ssl;
            var logReq = new LoginRequest(connection, token);

            var resp = await logReq.GetResponse();
            var session = new Session(host, resp.SessionId);
            session.Ssl = ssl;
            return session;
        }

        public static Task<LogOutResult> Logout(Session session)
        {
            var ltReq = new LogOutRequest(session);
            return ltReq.GetResponse();
        }

        public static Task<Session> Login( this ( string host, string token ) data, bool ssl = false )
            => Login(data.host, data.token, ssl );
    }
}