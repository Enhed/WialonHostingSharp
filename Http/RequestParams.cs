namespace WialonHostingSharp.Http
{
    public class RequestParams : IRequestParams
    {
        public static IRequestParams Empty => new RequestParams();
    }

    public interface IRequestParams
    {

    }
}