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
        public UpdateDeviceTypeRequest(Session connection, Params parameters) : base(connection, parameters)
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

    public sealed class RetranslatorUpdateObjectsRequest : Request<Retranslator>
    {

        public RetranslatorUpdateObjectsRequest(Session connection, Params parameters)
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

    public sealed class ImportMessageRequest : Request<string>
    {
        public ImportMessageRequest(Session session, Params parameters, Stream dataStream)
            : base(session, parameters)
        {
            this.dataStream = new MemoryStream();
            dataStream.CopyTo(this.dataStream);
        }

        private readonly Stream dataStream;

        public override string Method => "exchange/import_messages";
        protected override string HttpMethod => "POST";

        public override async Task<string> GetResponse()
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
                var response = await client.PostAsync(url, multipart);
                dataStream.Close();
                return await response.Content.ReadAsStringAsync();
            }
        }

        protected override string Convert(string source) => source;

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

    public sealed class GetFuelSettingRequest : Request<FuelSetting>
    {
        public GetFuelSettingRequest(Session connection, Params parameters)
            : base(connection, parameters)
        {
        }

        public override string Method => "unit/get_fuel_settings";

        public sealed class Params : RequestParams
        {
            [JsonProperty("itemId")]
            public long Id;
        }
    }

    public class FuelSetting
    {
        [JsonProperty("calcTypes")]
        public CalculateTypes CalcTypes;

        [JsonProperty("fuelLevelParams")]
        public FuelLevelParams FuelLevelParams;

        public enum CalculateTypes : uint
        {
            None,

            /// <summary>
            /// расход по расчету
            /// </summary>
            ConsumptionByCalculation,
            FuelLevelSensors,

            /// <summary>
            /// заменять ошибочные значения датчиков уровня топлива рассчитанными математически
            /// </summary>
            ErrorCalculatesMath,
            AbsoluteFuelSensors,
            ImpulseSensors,

            /// <summary>
            /// датчики мгновенного расхода топлива
            /// </summary>
            InstantaneousFuelConsumptionSensors,

            /// <summary>
            /// расход по нормам
            /// </summary>
            ConsumptionByRate
        }
    }

    public class FuelLevelParams
    {
        [JsonProperty("flags")]
        public uint Flags;

        [JsonProperty("ignoreStayTimeout")]
        public uint IgnoreStayTimeout;

        [JsonProperty("minFillingVolume")]
        public double MinFillingVolume;

        [JsonProperty("minTheftTimeout")]
        public uint MinTheftTimeout;

        [JsonProperty("minTheftVolume")]
        public double MinTheftVolume;

        [JsonProperty("filterQuality")]
        public byte FilterQuality;

        [JsonProperty("fillingsJoinInterval")]
        public uint FillingsJoinInterval;

        [JsonProperty("theftsJoinInterval")]
        public uint TheftsJoinInterval;

        [JsonProperty("extraFillingTimeout")]
        public uint ExtraFillingTimeout;
    }

    public sealed class UpdateFuelLevelParams : Request<bool>
    {
        private Session session;

        public UpdateFuelLevelParams(Session connection, Params parameters)
            : base(connection, parameters)
        {
        }

        public UpdateFuelLevelParams(Session session, long id, FuelLevelParams levelParams)
            : base(session, Params.Create(id, levelParams))
        {
        }

        public override string Method => "unit/update_fuel_level_params";

        protected override bool Convert(string source)
        {
            return source.Trim() == "{}";
        }

        public sealed class Params : FuelLevelParams, IRequestParams
        {
            [JsonProperty("itemId")]
            public long Id;

            public static Params Create(long id, FuelLevelParams source)
            {
                return new Params
                {
                    Id = id,
                    ExtraFillingTimeout = source.ExtraFillingTimeout,
                    FillingsJoinInterval = source.FillingsJoinInterval,
                    FilterQuality = source.FilterQuality,
                    Flags = source.Flags,
                    IgnoreStayTimeout = source.IgnoreStayTimeout,
                    MinFillingVolume = source.MinFillingVolume,
                    MinTheftTimeout = source.MinTheftTimeout,
                    MinTheftVolume = source.MinTheftVolume,
                    TheftsJoinInterval = source.TheftsJoinInterval
                };
            }
        }
    }

    public sealed class GetTripDetectorRequest : Request<TripDetector>
    {
        public GetTripDetectorRequest(Session connection, Params parameters)
            : base(connection, parameters)
        {
        }

        public GetTripDetectorRequest(Session connection, long id)
            : base(connection, new Params { Id = id })
        {
        }

        public override string Method => "unit/get_trip_detector";

        public sealed class Params : RequestParams
        {
            [JsonProperty("itemId")]
            public long Id;
        }
    }

    public sealed class UpdateTripDetectorRequest : Request<bool>
    {
        public UpdateTripDetectorRequest(Connection connection, Params parameters)
            : base(connection, parameters)
        {
        }

        public UpdateTripDetectorRequest(Connection connection, long id, TripDetector detector)
            : base(connection, Params.Create(id, detector))
        {
        }

        public override string Method => "unit/update_trip_detector";

        protected override bool Convert(string source) => source.Trim() == "{}";

        public sealed class Params : TripDetector, IRequestParams
        {
            [JsonProperty("itemId")]
            public long Id;

            public static Params Create(long id, TripDetector detector)
            {
                return new Params
                {
                    Id = id,
                    GpsCorrection = detector.GpsCorrection,
                    MaxMessagesMetters = detector.MaxMessagesMetters,
                    MinMovingSpeed = detector.MinMovingSpeed,
                    MinSat = detector.MinSat,
                    MinStaySeconds = detector.MinStaySeconds,
                    MinTripMetters = detector.MinTripMetters,
                    MinTripSeconds = detector.MinTripSeconds,
                    Type = detector.Type
                };
            }
        }
    }

    public class TripDetector
    {
        [JsonProperty("type")]
        public TypeDetector Type;

        [JsonProperty("gpsCorrection")]
        [JsonConverter(typeof(NumberBoolConverter))]
        public bool GpsCorrection;

        [JsonProperty("minSat")]
        public uint MinSat;

        [JsonProperty("minMovingSpeed")]
        public uint MinMovingSpeed;

        [JsonProperty("minStayTime")]
        public uint MinStaySeconds;

        [JsonProperty("maxMessagesDistance")]
        public uint MaxMessagesMetters;

        [JsonProperty("minTripTime")]
        public uint MinTripSeconds;

        [JsonProperty("minTripDistance")]
        public uint MinTripMetters;

        public enum TypeDetector : uint
        {
            Speed = 1,
            Position,
            IgnitionSensor,
            MileageSensor,
            RelativeOdometer
        }
    }

        public sealed class TokenListRequest : Request<Token[]>
    {
        public TokenListRequest(Session connection, Params parameters)
            : base(connection, parameters)
        {
        }

        public TokenListRequest(Session connection)
            : base(connection, new Params())
        {
        }

        public override string Method => "token/list";

        public sealed class Params : RequestParams
        {
            [JsonProperty("userId", NullValueHandling = NullValueHandling.Ignore)]
            public string UserId;
        }
    }

    public sealed class Token
    {
        [JsonProperty("h")]
        public string Value;

        [JsonProperty("app")]
        public string Application;

        [JsonProperty("at")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime ActivationUtcDate;

        [JsonProperty("ct")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime CreatedUtcDate;

        [JsonProperty("dur")]
        public uint DurationSeconds;

        [JsonProperty("fl")]
        public long Flags;

        [JsonProperty("items")]
        public long[] AllowItemsId;

        [JsonProperty("p")]
        public string CustomParameters;

        [JsonIgnore]
        public DateTime ExpiredUtcDate => CreatedUtcDate.AddSeconds(DurationSeconds);

        [JsonIgnore]
        public TimeSpan DurationTime => TimeSpan.FromSeconds(DurationSeconds);

        [JsonIgnore]
        public bool IsExpired => ExpiredUtcDate.ToLocalTime() > DateTime.Now;
    }
}
