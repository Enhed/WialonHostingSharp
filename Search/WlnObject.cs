using Newtonsoft.Json;
using WialonHostingSharp.Messages;
using WialonHostingSharp.Http;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using SharpExtension;

namespace WialonHostingSharp.Search
{
    public class WlnObject : Unit
    {
        private static ProtocolInfo[] protocols;

        [JsonIgnore]
        public Session Session;

        [JsonProperty("uacl")]
        public string UserAccessLevel;

        [JsonProperty("uid")]
        public string Uid;

        [JsonProperty("uid2")]
        public string Uid2;

        [JsonProperty("hw")]
        public long? HardwareType;

        [JsonProperty("ph")]
        public string Phone;

        [JsonProperty("ph2")]
        public string Phone2;

        [JsonProperty("psw")]
        public string Password;

        [JsonProperty("lmsg")]
        public DataMessage LastMessage;

        [JsonProperty("sens")]
        public Dictionary<string, Sensor> SensorsDictionary;

        public Sensor[] Sensors => SensorsDictionary?.Values?.ToArray();

        [JsonIgnore]
        public IEnumerable<Sensor> FuelSensors => Sensors?.Where(s => s.SensorType == Sensor.Type.FuelLevel);

        [JsonIgnore]

        public Task<FuelSetting> FuelSetting => GetFuelSetting(Session);

        [JsonIgnore]
        public Task<TripDetector> TripDetector => GetTripDetector(Session);

        public Task<TripDetector> GetTripDetector(Session session)
        {
            return new GetTripDetectorRequest
            (
                session,
                new GetTripDetectorRequest.Params { Id = Id }
            ).GetResponse();
        }

        public Task<bool> UpdateTripDetector(Session session, TripDetector detector)
        {
            return new UpdateTripDetectorRequest(session, Id, detector).GetResponse();
        }

        public Task<bool> UpdateTripDetector(TripDetector detector)
        {
            return new UpdateTripDetectorRequest(Session, Id, detector).GetResponse();
        }

        public Task<UpdateDeviceTypeRequest.Response> UpdateDevice(Session session, UpdateDeviceTypeRequest.Params parameters)
        {
            var req = new UpdateDeviceTypeRequest(session, parameters);
            return req.GetResponse();
        }

        public Task<UpdateDeviceTypeRequest.Response> UpdateDevice(Session session, string uid, long device)
        {
            var prms = new UpdateDeviceTypeRequest.Params
            {
                Id = Id,
                Uid = uid,
                DeviceType = device
            };

            var req = new UpdateDeviceTypeRequest(session, prms);
            return req.GetResponse();
        }

        public Task<UpdateDeviceTypeRequest.Response> ChangeUid(Session session, string uid)
        {
            if(HardwareType == null) throw new Exception($"{nameof(HardwareType)} is null, use method with parameters!");

            var req = new UpdateDeviceTypeRequest
            (
                session,
                new UpdateDeviceTypeRequest.Params
                {
                    Id = Id,
                    Uid = uid,
                    DeviceType = HardwareType.Value
                }
            );

            return req.GetResponse();
        }

        public Task<FuelSetting> GetFuelSetting(Session session)
        {
            return new GetFuelSettingRequest
            (
                session,
                new GetFuelSettingRequest.Params { Id = Id }
            ).GetResponse();
        }

        public Task<bool> UpdateFuelLevelParams(Session session, FuelLevelParams levelParams)
        {
            return new UpdateFuelLevelParams(session, Id,levelParams).GetResponse();
        }

        public async Task<bool> UpdateFuelLevelParams(Session session, Action<FuelLevelParams> updater)
        {
            var fset = await GetFuelSetting(session);
            updater(fset.FuelLevelParams);
            return await UpdateFuelLevelParams(session, fset.FuelLevelParams);
        }

        public Task<UpdateDeviceTypeRequest.Response> ChangeDevice(Session session, long device)
        {
            if(HardwareType == null) throw new Exception($"{nameof(Uid)} is null, use method with parameters!");

            var req = new UpdateDeviceTypeRequest
            (
                session,
                new UpdateDeviceTypeRequest.Params
                {
                    Id = Id,
                    Uid = Uid,
                    DeviceType = device
                }
            );

            return req.GetResponse();
        }

        public async Task<ProtocolInfo> GetProtocol(Session session)
        {
            if(protocols == null)
            {
                protocols = await new ProtocolInfoRequest
                (
                    session,
                    new ProtocolInfoRequest.Params
                    {
                    }
                ).GetResponse();
            }

            return protocols.FirstOrDefault(pi => pi.Id == HardwareType);
        }

        public Task<DataMessage[]> GetDataMessages(MessageService ms, DateTime begin, DateTime end)
        {
            return ms.GetDataMessages(Id, begin, end);
        }

        public Task<DataMessage[]> GetDataMessages(MessageService ms, DateTime begin)
        {
            return GetDataMessages(ms, begin, DateTime.Now);
        }

        public Task<DataMessage[]> GetDataMessages(Session session, DateTime begin, DateTime end)
        {
            return GetDataMessages(new MessageService(session), begin, end);
        }

        public Task<DataMessage[]> GetDataMessages(Session session, DateTime begin)
        {
            return GetDataMessages(new MessageService(session), begin);
        }

        public async Task Rename(Session session, string name)
        {
            var result = await new ItemUpdateNameRequest
            (
                session,
                new ItemUpdateNameRequest.Params { Id = Id, Name = name }
            ).GetResponse();

            if(result.Name != name) throw new Exception($"Cannot change name [{Name}] => [{name}] on id [{Id}]. Response: {result}");
        }
    }

    public class Sensor : ICloneable
    {
        [JsonProperty("id")]
        public long Id;

        [JsonProperty("n")]
        public string Name;

        [JsonProperty("t")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Type SensorType;

        [JsonProperty("d")]
        public string Description;

        [JsonProperty("p")]
        public string Parameter;

        [JsonProperty("f")]
        public uint Flags;

        [JsonProperty("c")]
        public string Configuration;

        [JsonProperty("vt")]
        public uint ValidationType;

        [JsonProperty("vs")]
        public uint ValidationSensorId;

        [JsonProperty("m")]
        public string Measure = string.Empty;

        [JsonProperty("tbl")]
        public TableUnit[] Table = new TableUnit[0];

        public Task<Sensor> Update(Session session, long itemId)
        {
            return new SensorUpdateRequest
            (
                session,
                new SensorUpdateRequest.Params(this)
                {
                    ItemId = itemId
                }
            ).GetResponse();
        }

        public Task<Sensor> Create(Session session, long itemId)
        {
            return new SensorUpdateRequest
            (
                session,
                new SensorUpdateRequest.Params(this)
                {
                    ItemId = itemId,
                    CallMode = SensorUpdateRequest.CallMode.Create,
                    Id = 0
                }
            ).GetResponse();
        }

        public Task<Sensor> Delete(Session session, long itemId, uint unLink = 1)
        {
            return new SensorUpdateRequest
            (
                session,
                new SensorUpdateRequest.Params(this)
                {
                    ItemId = itemId,
                    UnLink = unLink,
                    CallMode = SensorUpdateRequest.CallMode.Delete
                }
            ).GetResponse();
        }

        public virtual object Clone()
        {
            return new Sensor
            {
                Configuration = this.Configuration,
                Description = this.Description,
                Flags = this.Flags,
                Id = this.Id,
                Measure = this.Measure,
                Name = this.Name,
                Parameter = this.Parameter,
                SensorType = this.SensorType,
                Table = this.Table,
                ValidationSensorId = this.ValidationSensorId,
                ValidationType = this.ValidationType
            };
        }

        public sealed class TableUnit
        {
            [JsonProperty("x")]
            public double X;

            [JsonProperty("a")]
            public double A;

            [JsonProperty("b")]
            public double B;
        }

        /// <summary>
        /// Типы датчиков
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// датчик абсолютного расхода топлива
            /// </summary>
            [EnumMember(Value="absolute fuel consumption")]
            AbsoluteFuelConsumption,

            /// <summary>
            /// акселерометр
            /// </summary>
            [EnumMember(Value="accelerometer")]
            Accelerometer,

            /// <summary>
            /// тревожная кнопка
            /// </summary>
            [EnumMember(Value="alarm trigger")]
            AlarmTrigger,

            /// <summary>
            /// счетчик
            /// </summary>
            [EnumMember(Value="counter")]
            Counter,

            /// <summary>
            /// произвольный датчик
            /// </summary>
            [EnumMember(Value="custom")]
            Custom,

            /// <summary>
            /// произвольный цифровой датчик
            /// </summary>
            [EnumMember(Value="digital")]
            Digital,

            /// <summary>
            /// привязка водителя
            /// </summary>
            [EnumMember(Value="driver")]
            Driver,

            /// <summary>
            /// датчик полезной работы двигателя
            /// </summary>
            [EnumMember(Value="engine efficiency")]
            EngineEfficiency,

            /// <summary>
            /// абсолютные моточасы
            /// </summary>
            [EnumMember(Value="engine hours")]
            EngineHours,

            /// <summary>
            /// датчик зажигания
            /// /// </summary>
            [EnumMember(Value="engine operation")]
            EngineOperation,

            /// <summary>
            /// датчик оборотов двигателя
            /// </summary>
            [EnumMember(Value="engine rpm")]
            EngineRpm,

            /// <summary>
            /// импульсный датчик уровня топлива
            /// </summary>
            [EnumMember(Value="fuel level impulse sensor")]
            FuelLevelImpulsesensor,

            /// <summary>
            /// датчик уровня топлива
            /// </summary>
            [EnumMember(Value="fuel level")]
            FuelLevel,

            /// <summary>
            /// импульсный датчик расхода топлива
            /// </summary>
            [EnumMember(Value="impulse fuel consumption")]
            ImpulseFuelConsumption,

            /// <summary>
            /// датчик мгновенного расхода топлива
            /// </summary>
            [EnumMember(Value="instant fuel consumption")]
            InstantFuelConsumption,

            /// <summary>
            /// датчик пробега
            /// </summary>
            [EnumMember(Value="mileage")]
            Mileage,

            /// <summary>
            /// относительный одометр
            /// </summary>
            [EnumMember(Value="odometer")]
            Odometer,

            /// <summary>
            /// частный режим
            /// </summary>
            [EnumMember(Value="private mode")]
            PrivateMode,

            /// <summary>
            /// относительные моточасы
            /// </summary>
            [EnumMember(Value="relative engine hours")]
            RelativeEngineHours,

            /// <summary>
            /// коэффициент температуры
            /// </summary>
            [EnumMember(Value="temperature coefficient")]
            TemperatureCoefficient,

            /// <summary>
            /// датчик температуры
            /// </summary>
            [EnumMember(Value="temperature")]
            Temperature,

            /// <summary>
            /// привязка прицепа
            /// </summary>
            [EnumMember(Value="trailer")]
            Trailer,

            /// <summary>
            /// датчик напряжения
            /// </summary>
            [EnumMember(Value="voltage")]
            Voltage,

            /// <summary>
            /// датчик веса
            /// </summary>
            [EnumMember(Value="weight sensor")]
            WeightSensor
        }
    }

    public class SensorUpdateRequest : Request<Sensor>
    {
        public SensorUpdateRequest(Session connection, Params parameters)
            : base(connection, parameters)
        {

        }

        public override string Method => "unit/update_sensor";

        protected override Sensor Convert(string source)
        {
            try
            {
                var jarr = JArray.Parse(source);
                var jsource = jarr[1].ToString();
                return JsonConvert.DeserializeObject<Sensor>(jsource);
            }
            catch(Exception ex)
            {
                throw new Exception($"Unexcepted response [{source}] with request [{FullUrl}]", ex);
            }
        }

        public sealed class Params : Sensor, IRequestParams
        {
            public Params() {}

            public Params(Sensor sensor)
            {
                this.Name = sensor.Name;
                this.Id = sensor.Id;
                this.Description = sensor.Description;
                this.Flags = sensor.Flags;
                this.Measure = sensor.Measure;
                this.Parameter = sensor.Parameter;
                this.Table = sensor.Table;
                this.SensorType = sensor.SensorType;
                this.ValidationSensorId = sensor.ValidationSensorId;
                this.ValidationType = sensor.ValidationType;
                this.Configuration = sensor.Configuration;
            }

            [JsonProperty("itemId")]
            public long ItemId;

            [JsonProperty("callMode")]
            [JsonConverter(typeof(StringEnumConverter))]
            public CallMode CallMode = CallMode.Update;

            [JsonProperty("unlink", NullValueHandling = NullValueHandling.Ignore)]
            public uint? UnLink;
        }



        public enum CallMode
        {
            [EnumMember(Value="create")]
            Create,

            [EnumMember(Value="update")]
            Update,

            [EnumMember(Value="delete")]
            Delete
        }
    }
}