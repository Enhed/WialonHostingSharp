using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System;
using WialonHostingSharp.Search;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Net.Http;
using SharpExtension;

namespace WialonHostingSharp.Http
{
    public abstract class Request<T>
    {
        protected Request(Connection connection, IRequestParams parameters)
        {
            Connection = connection;
            RequestParams = parameters;
        }

        public readonly Connection Connection;
        public readonly IRequestParams RequestParams;
        public string JsonRequestParams => JsonConvert.SerializeObject(RequestParams);
        public abstract string Method { get; }
        public string FullUrl => Connection.GenerateUrl( Method, JsonRequestParams );
        public string FullUrlWithoutParameters => Connection.GenerateUrl(Method);

        protected virtual string HttpMethod => "GET";

        public virtual Task<T> GetResponse()
        {
            return GetResponse(GetWebRequest());
        }

        public async Task<T> GetResponse(WebRequest request)
        {
            using(var webResponse = await request.GetResponseAsync())
            {
                using(var stream = webResponse.GetResponseStream())
                {
                    using(var reader = new StreamReader(stream))
                    {
                        var text = await reader.ReadToEndAsync();
                        return Convert(text);
                    }
                }
            }
        }

        public WebRequest GetWebRequest()
        {
            var request = WebRequest.Create(FullUrl);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = HttpMethod;
            return request;
        }

        public Task<WebResponse> GetWebResponse()
        {
            return GetWebRequest().GetResponseAsync();
        }

        protected virtual T Convert(string source) => JsonConvert.DeserializeObject<T>(source);
    }

        public sealed class ItemUpdateNameRequest : Request<ItemUpdateNameRequest.Response>
    {
        public ItemUpdateNameRequest(Session connection,
            Params parameters) : base(connection, parameters)
        {
        }

        public ItemUpdateNameRequest(Session connection, long id, string name)
            : this(connection, new Params{ Id = id, Name = name })
        {

        }

        public override string Method => "item/update_name";

        public sealed class Params : RequestParams
        {
            [JsonProperty("itemId")]
            public long Id;


            [JsonProperty("name")]
            public string Name;
        }

        public sealed class Response
        {
            [JsonProperty("nm")]
            public string Name;
        }
    }

    public sealed class UpdateDeviceTypeRequest : Request<UpdateDeviceTypeRequest.Response>
    {
        public UpdateDeviceTypeRequest(Session connection, RequestParams parameters) : base(connection, parameters)
        {
        }

        public override string Method => "unit/update_device_type";

        public sealed class Params : RequestParams
        {
            [JsonProperty("itemId")]
            public long Id;

            [JsonProperty("deviceTypeId")]
            public long DeviceType;

            [JsonProperty("uniqueId")]
            public string Uid;
        }

        public sealed class Response
        {
            [JsonProperty("uid")]
            public string Uid;

            [JsonProperty("hw")]
            public long Type;
        }
    }

    public sealed class CreateObjectRequest : Request<CreateObjectResponse>
    {
        public CreateObjectRequest(Session connection, Params parameters) : base(connection, parameters)
        {
        }

        public override string Method => "core/create_unit";

        public sealed class Params : RequestParams
        {
            [JsonProperty("creatorId")]
            public long CreatorId;

            [JsonProperty("name")]
            public string Name;

            [JsonProperty("hwTypeId")]
            public long Procotol;

            [JsonProperty("dataFlags")]
            public long Flags = 1;
        }
    }

    public sealed class CreateObjectResponse
    {
        [JsonProperty("item")]
        public WlnObject Object;

        [JsonProperty("flags")]
        public uint Flags;
    }

    public sealed class UpdateObjectsRequest : Request<Retranslator>
    {

        public UpdateObjectsRequest(Session connection, Params parameters)
            : base(connection, parameters)
        {
        }

        public override string Method => "retranslator/update_units";

        public sealed class Params : RequestParams
        {
            [JsonProperty("itemId")]
            public long Id;

            [JsonProperty("units")]
            public Retranslator.Object[] Objects;

            [JsonProperty("callMode")]
            [JsonConverter(typeof(StringEnumConverter))]
            public Mode Mode;
        }

        public enum Mode
        {
            [EnumMember(Value = "add")]
            Add,

            [EnumMember(Value = "remove")]
            Remove
        }

    }
    
    public sealed class ProtocolInfoRequest : Request<ProtocolInfo[]>
    {
        public ProtocolInfoRequest(Session connection, Params parameters) : base(connection, parameters)
        {
        }

        public override string Method => "core/get_hw_types";

        public sealed class Params : RequestParams
        {
            [JsonProperty("filterType", NullValueHandling = NullValueHandling.Ignore)]
            [JsonConverter(typeof(StringEnumConverter))]
            public FilterType Filter = FilterType.Name;

            [JsonProperty("filterValue", NullValueHandling = NullValueHandling.Ignore)]
            public string Mask = "*";

            [JsonProperty("includeType")]
            public bool IncludeType = true;

            
        }

        public enum FilterType
        {
            [EnumMember(Value = "name")]
            Name,

            [EnumMember(Value = "id")]
            Id,

            [EnumMember(Value = "type")]
            Type
        }
    }

    public sealed class ProtocolInfo
    {
        [JsonProperty("id")]
        public long Id;

        [JsonProperty("uid2")]
        public uint Id2;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("hw_category")]
        public string Category;

        [JsonProperty("tp")]
        public string TCP;

        [JsonProperty("up")]
        public string UDP;
    }

    public sealed class ImportMessageRequest : Request<ImportMessageResponse>
    {
        public ImportMessageRequest(Session session, Params parameters, Stream dataStream, bool autoCloseStream = true)
            : base(session, parameters)
        {
            this.dataStream = dataStream;
            this.autoCloseStream = autoCloseStream;
        }

        private readonly Stream dataStream;
        private readonly bool autoCloseStream;

        public override string Method => "exchange/import_messages";
        protected override string HttpMethod => "POST";

        public override async Task<ImportMessageResponse> GetResponse()
        {
            var client = new HttpClient();
            var url = FullUrl;

            var multipart = new MultipartFormDataContent("----WebKitFormBoundarylvunQiir9AesO8qB");

            multipart.Add
            (
                new StringContent(((ImportMessageRequest.Params)this.RequestParams).EventHash)
                    .Do(x => x.Headers.Add("Content-Disposition", "form-data; name=\"eventHash\""))
            );

            multipart.Add
            (
                new StreamContent(dataStream)
                    .Do(x => x.Headers.Add("Content-Disposition", "form-data; name=\"messages_filter_import_file\"; filename=\"4100.zip\""))
                    .Do(x => x.Headers.Add("Content-Type", "application/zip"))
            );

            using(client)
            {
                return (await(await client.PostAsync(url, multipart))
                    .Content.ReadAsStringAsync()).Get(Convert);
            }
        }

        public sealed class Params : RequestParams
        {
            [JsonProperty("itemId")]
            public long Id;

            [JsonProperty("eventHash", NullValueHandling = NullValueHandling.Ignore)]
            public string EventHash;
        }
    }

    public sealed class ImportMessageResponse
    {

    }

    public sealed class CreateMessagesLayerRequest : Request<CreateMessagesLayerResponse>
    {
        public CreateMessagesLayerRequest(Session session, Params parameters) : base(session, parameters)
        {
        }

        public override string Method => "render/create_messages_layer";

        public sealed class Params : RequestParams
        {
            [JsonProperty("layerName")]
            public string Name;

            [JsonProperty("itemId")]
            public long Id;

            [JsonProperty("timeFrom")]
            [JsonConverter(typeof(UnixDateTimeConverter))]
            public DateTime From;

            [JsonProperty("timeTo")]
            [JsonConverter(typeof(UnixDateTimeConverter))]
            public DateTime To = DateTime.Now;

            [JsonProperty("tripDetector")]
            [JsonConverter(typeof(NumberBoolConverter))]
            public bool TripDetector;

            [JsonProperty("trackColor")]
            public string TrackColor = "0004ff";

            [JsonProperty("trackWidth")]
            public int TrackWidth = 1;

            [JsonProperty("arrows")]
            [JsonConverter(typeof(NumberBoolConverter))]
            public bool Arrows;

            [JsonProperty("points")]
            [JsonConverter(typeof(NumberBoolConverter))]
            public bool Points;

            [JsonProperty("pointColor")]
            public string PointColor = "0004ff";

            [JsonProperty("annotations")]
            [JsonConverter(typeof(NumberBoolConverter))]
            public bool Annotations;

            [JsonProperty("flags", NullValueHandling = NullValueHandling.Ignore)]
            public Params.FLag? Flags;

            public enum FLag : uint
            {
                GroupingMarkers = 0x0001,
                NumberingMarkers = 0x0002,
                EventMarkers = 0x0004,
                Fueling = 0x0008,
                GetImage = 0x0010,
                Parkings = 0x0020,
                OverSpeed = 0x0040,
                Stops = 0x0080,
                FuelDischarge = 0x0100,
                VideoMarkers = 0x0800
            }
        }
    }

    public sealed class CreateMessagesLayerResponse
    {
        [JsonProperty("name")]
        public string Name;
    }

    public sealed class ExportMessagesRequest : Request<string>
    {
        public ExportMessagesRequest(Session session, Params parameters)
            : base(session, parameters)
        {
        }

        public override string Method => "exchange/export_messages";

        public sealed class Params : RequestParams
        {
            [JsonProperty("layerName", NullValueHandling = NullValueHandling.Ignore)]
            public string LayerName;

            [JsonProperty("format")]
            [JsonConverter(typeof(StringEnumConverter))]
            public ExportFormat Format = ExportFormat.Wln;

            [JsonProperty("timeFrom", NullValueHandling = NullValueHandling.Ignore)]
            [JsonConverter(typeof(UnixDateTimeConverter))]
            public DateTime? From;

            [JsonProperty("timeTo", NullValueHandling = NullValueHandling.Ignore)]
            [JsonConverter(typeof(UnixDateTimeConverter))]
            public DateTime? To;

            [JsonProperty("compress")]
            [JsonConverter(typeof(NumberBoolConverter))]
            public bool Compress = false;
        }
    }

    public sealed class NumberBoolConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((bool)value) ? 1 : 0);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value.ToString() == "1";
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }
    }

    public enum ExportFormat
    {
        [EnumMember(Value="txt")]
        Txt,

        [EnumMember(Value="kml")]
        Kml,

        [EnumMember(Value="plt")]
        Plt,

        [EnumMember(Value="wln")]
        Wln,

        [EnumMember(Value="wlb")]
        Wlb
    }
}
