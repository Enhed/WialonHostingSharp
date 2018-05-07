using WialonHostingSharp;

namespace WialonHostingSharp
{
    public sealed class FindElementsRequest<T> : Request<FindElementsResult<T>>
        where T : Unit
    {
        public FindElementsRequest(Session connection, FindElementsParams parameters) : base(connection, parameters)
        {
        }

        public override string Method => "core/search_items";

        //protected override string Convert(string source) => source;
    }

}