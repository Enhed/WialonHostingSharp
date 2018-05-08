using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System;

namespace WialonHostingSharp.Http
{
    public abstract class Request<T>
    {
        protected Request(Connection connection, RequestParams parameters)
        {
            Connection = connection;
            RequestParams = parameters;
        }

        public readonly Connection Connection;
        public readonly RequestParams RequestParams;
        public abstract string Method { get; }
        public string FullUrl => Connection.GenerateUrl( Method, JsonConvert.SerializeObject(RequestParams));

        public virtual async Task<T> GetResponse()
        {
            var request = WebRequest.Create(FullUrl);
            request.ContentType = "application/x-www-form-urlencoded";

            using(var response =  await request.GetResponseAsync())
            {
                using(var stream = response.GetResponseStream())
                {
                    using(var reader = new StreamReader(stream))
                    {
                        var text = await reader.ReadToEndAsync();
                        return Convert(text);
                    }
                }
            }
            
        }

        protected virtual T Convert(string source) => JsonConvert.DeserializeObject<T>(source);
    }
}