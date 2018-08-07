using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace WialonHostingSharp.Messages
{
    public sealed class DataMessage : Message
    {
        [JsonProperty("pos")]
        public Position Position;

        [JsonProperty("i")]
        public uint DataIn;

        [JsonProperty("o")]
        public uint DataOut;

        [JsonProperty("p")]
        public Dictionary<string, string> Parameters;

        public GalileSkyDataMessage ToGalileoSky() => new GalileSkyDataMessage { DataMessage = this };
    }

    public sealed class GalileSkyDataMessage
    {
        public DataMessage DataMessage;

        public bool IsGpsAntennaOff => (DataMessage.DataOut & (1 << 22)) > 0;
    }

    public static class DataMessageExtension
    {
        public static IEnumerable<DataMessage> GetInvalids(this IEnumerable<DataMessage> source)
            => source.Where(dm => !dm.Position?.IsValid ?? true);

        public static IEnumerable<DataMessage> GetValids(this IEnumerable<DataMessage> source)
            => source.Where(dm => dm.Position?.IsValid ?? false);
    }
}