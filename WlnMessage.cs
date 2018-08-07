using System.Linq;
using SharpExtension;
using SharpExtension.Generic;

using System;
using WialonHostingSharp.Messages;
using System.Collections.Generic;

namespace WialonHostingSharp
{
    public sealed class WlnMessage
    {
        private const string FirstValue = "REG";
        private const int RequireLen = 11;

        private const int DateIndex = 1;
        private const int LatitudeIndex = 2;
        private const int LongitudeIndex = 3;
        private const int SpeedIndex = 4;
        private const int CourseIndex = 5;
        private const int DataIndex = 6;
        private const int SystemDataIndex = DataIndex + 1;
        private string DataSeparator = ",";
        private const string ValueSeparator = ":";
        private const string BlockSeparator = ";";
        private const string AltitudeName = "ALT";
        private const string SatelliteCountName = "SATS";

        private readonly string[] sourceArray;

        public WlnMessage(DateTime time)
        {
            sourceArray = new string[RequireLen];
            sourceArray[0] = FirstValue;
            sourceArray[DateIndex] = time.ToEpoch().ToString();
        }

        public WlnMessage(DataMessage message) : this(message.UtcTime)
        {
            Position = message.Position;

            message.Parameters
                .Each(kv => SetOrAddDataValue(kv.Key, kv.Value));
        }

        public WlnMessage(string sourceMessage)
        {
            sourceArray = sourceMessage.Split(BlockSeparator);
        }

        public Position Position
        {
            get
            {
                if(sourceArray[LongitudeIndex].IsNullOrWhiteSpace()) return null;
                if(sourceArray[LatitudeIndex].IsNullOrWhiteSpace()) return null;
                
                var lat = double.Parse( sourceArray[LatitudeIndex] );
                var lon = double.Parse( sourceArray[LongitudeIndex] );

                var altSrc = GetDataValue(AltitudeName);
                var satsSrc = GetSystemDataValue(SatelliteCountName);
                var spSrc = sourceArray[SpeedIndex];
                var courceSrc = sourceArray[CourseIndex];

                var alt = altSrc.IsNullOrWhiteSpace() ? 0 : double.Parse(altSrc);
                var cource = courceSrc.IsNullOrWhiteSpace() ? 0 : uint.Parse( courceSrc );
                var sats = satsSrc.IsNullOrWhiteSpace() ? byte.MinValue : byte.Parse(satsSrc);
                var sp = spSrc.IsNullOrWhiteSpace() ? 0 : uint.Parse( spSrc );

                return new Position
                {
                    Altitude = alt, Course = cource, Latitude = lat,
                    Longitude = lon, SatelliteCount = sats, Speed = sp
                };
            }

            set
            {
                if(value == null) return;

                sourceArray[LongitudeIndex] = value.Longitude.ToString();
                sourceArray[LatitudeIndex] = value.Latitude.ToString();
                sourceArray[SpeedIndex] = value.Speed.ToString();
                sourceArray[CourseIndex] = value.Course.ToString();

                SetOrAddDataValue(AltitudeName, value.Altitude.ToString());
                SetOrAddSystemValue(SatelliteCountName, value.SatelliteCount.ToString());
            }
        }

        public DateTime UtcTime
        {
            get => long.Parse(sourceArray[DateIndex]).ToDateTime();
            set => sourceArray[DateIndex] = value.ToEpoch().ToString();
        }
        public DateTime LocalTime
        {
            get => UtcTime.ToLocalTime();
            set => UtcTime = value.ToUniversalTime();
        }

        public IEnumerable<(string name, string value)> Data => GetSplittedValues(sourceArray[DataIndex]);
        public IEnumerable<(string name, string value)> SystemData => GetSplittedValues(sourceArray[SystemDataIndex]);

        private IEnumerable<(string name, string value)> GetSplittedValues(string data)
            => data?.Split(DataSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Select(str => str.Split(ValueSeparator, StringSplitOptions.RemoveEmptyEntries))
                .Select(ar => (ar[0], ar[1]));

        public string GetDataValue(string name)
            => Data?.FirstOrDefault(x => x.name == name).value;

        public string GetSystemDataValue(string name)
            => SystemData?.FirstOrDefault(x => x.name == name).value;

        public void SetOrAddDataValue(string name, string value)
            => sourceArray[DataIndex] = GetSettedOrAddedValues(DataIndex, name, value);

        public void ChangeDataValue(string name, Func<string, string> changeTo)
        {
            var x = GetDataValue(name);
            if(x.IsNullOrWhiteSpace()) return;
            var newValue = changeTo(x);
            SetOrAddDataValue(name, newValue);
        }

        public void SetOrAddSystemValue(string name, string value)
            => sourceArray[SystemDataIndex] = GetSettedOrAddedValues(SystemDataIndex, name, value);

        public void ChangeSystemValue(string name, Func<string, string> changeTo)
        {
            var x = GetSystemDataValue(name);
            if(x.IsNullOrWhiteSpace()) return;
            SetOrAddSystemValue(name, changeTo(x));
        }

        public void ReplaceInDataSource(string oldValue, string newValue)
        {
            sourceArray[DataIndex] = sourceArray[DataIndex].Replace(oldValue, newValue);
            sourceArray[SystemDataIndex] = sourceArray[SystemDataIndex].Replace(oldValue, newValue);
        }

        public string GetSettedOrAddedValues(int index, string name, string value)
        {
            var sourceValues = sourceArray[index];
            var splitted = GetSplittedValues(sourceValues);
            var current = splitted?.FirstOrDefault(x => x.name == name).value;
            var prefix = splitted?.Count() > 0 ? DataSeparator : string.Empty;

            if(current.IsNullOrWhiteSpace()) return sourceValues + $"{prefix}{name}:{value}";
            else
            {
                return sourceArray[index].Replace($"{name}:{current}", $"{name}:{value}");
            }
        }

        public override string ToString() => sourceArray.Join(BlockSeparator);
    }
}