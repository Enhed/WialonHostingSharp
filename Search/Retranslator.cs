using Newtonsoft.Json;
using WialonHostingSharp.Search;

namespace WialonHostingSharp.Search
{
    public sealed class Retranslator : Unit
    {
        [JsonProperty("rtru", NullValueHandling = NullValueHandling.Ignore)]
        public Object[] Objects;

        [JsonProperty("rtro")]
        public int State;

        [JsonProperty("rtrc")]
        public Configuration Config;

        [JsonIgnore]
        public bool Enabled => State == 1;

#region types

        public enum Flags : long
        {
            Base = 1,
            CustomProperty = 2,
            Biling = 4,
            Guid = 64,
            Admin = 128,
            Config = 256,
            Objects = 512,
            All = Base | CustomProperty | Biling | Guid | Admin | Config | Objects
        }

        public sealed class Configuration
        {
            [JsonProperty("protocol")]
            public string Protocol;

            [JsonProperty("server")]
            public string Server;

            [JsonProperty("port")]
            public int Port;

            [JsonProperty("auth")]
            public string Auth;

            [JsonProperty("ssl")]
            public int StateSsl;

            [JsonProperty("v6type")]
            public int StateV6Type;

            [JsonProperty("login")]
            public string Login;

            [JsonProperty("password")]
            public string Password;

            [JsonProperty("notauth")]
            public int StateNotAuth;

            [JsonIgnore]
            public bool Ssl => StateSsl == 1;

            [JsonIgnore]
            public bool V6Type => StateV6Type == 1;

            [JsonIgnore]
            public bool NotAuth => StateNotAuth == 1;
        }

        public sealed class Object
        {
            [JsonProperty("i")]
            public long Id;

            [JsonProperty("a")]
            public string Uid;
        }

#endregion
    }
}