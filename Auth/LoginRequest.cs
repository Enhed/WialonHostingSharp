using WialonHostingSharp.Http;

namespace WialonHostingSharp.Auth
{
    public sealed class LoginRequest : Request<LoginResponse>
    {
        public LoginRequest(Connection connection, LoginParams parameters) : base(connection, parameters)
        {
        }

        public LoginRequest(Connection connection, string token)
            : base( connection, new LoginParams { Token = token } )
        {
        }

        public LoginRequest(Connection connection, string token, LoginFlags flag)
            : base( connection, new LoginParams { Token = token, Flag = flag } )
        {
        }

        public LoginRequest(Connection connection, string token, LoginFlags flag, string user)
            : base( connection, new LoginParams { Token = token, Flag = flag, User = user } )
        {
        }

        public override string Method => "token/login";
    }
}