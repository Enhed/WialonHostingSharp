using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SharpExtension;
using SharpExtension.Generic;
using WialonHostingSharp.Http;
using WialonHostingSharp.Search;

namespace WialonHostingSharp.Search
{
    public sealed class SearchService
    {
        private readonly Session session;

        public SearchService(Session session)
        {
            this.session = session;
        }

        public async Task<T[]> GetUnits<T>(SearchSpector searchSpector, uint force, long flags,
            uint from, uint to)
            where T : Unit
        {
            var param = new FindElementsParams
            {
                Spector = searchSpector,
                Force = force,
                Flags = flags,
                From = from,
                To = to
            };

            var req = new FindElementsRequest<T>(session, param);
            var result = await req.GetResponse();
            return result.Items;
        }

        public Task<User> GetUserById(int id, long flags = 1)
        {
            return GetUser(id.ToString(), PropertyElement.sys_id, flags);
        }

        public Task<Retranslator> GetRetranslatorById(int retranslatorId, long flags = 1)
        {
            return GetRetranslator(retranslatorId.ToString(), PropertyElement.sys_id, flags);
        }

        public Task<Retranslator> GetRetranslatorByName(string name, long flags = 1)
        {
            return GetRetranslator(name, PropertyElement.sys_name, flags);
        }

        public async Task<WlnObject[]> GetObjects(string mask = "*",
            PropertyElement propName = PropertyElement.sys_name, long flags = 1,
            uint force = 1, uint from = 0, uint to = 0)
        {
            var ss = new SearchSpector
            {
                ItemType = ItemType.Object,
                PropertyName = propName.ToString(),
                Mask = mask
            };

            return (await GetUnits<WlnObject>(ss, force, flags, from, to))
                .Each(o => o.Session = session).ToArray();
        }

        public async Task<WlnObject[]> GetObjects(long flags = 1, uint force = 1, uint from = 0, uint to = 0)
        {
            return (await GetObjects("*", PropertyElement.sys_name, flags, force, from, to))
                .Each(o => o.Session = session).ToArray();
        }

        public async Task<WlnObject> GetObject(string mask = "*",
            PropertyElement propName = PropertyElement.sys_name, long flags = 1,
            uint force = 1, uint from = 0, uint to = 0)
        {
            var objs = await GetObjects(mask, propName, flags, force, from, to);

            if(objs.Length == 0) return null;

            return objs?.Length > 1
                ? throw new AggregateException($"Mask [{mask}] get x{objs?.Length ?? -1} elements")
                : objs[0].Do(o => o.Session = session);
        }

        public async Task<Retranslator> GetRetranslator(string mask = "*",
            PropertyElement propName = PropertyElement.sys_name, long flags = 1,
            uint force = 1, uint from = 0, uint to = 0)
        {
            return (await GetRetranslators(mask, propName, flags, force, from, to))
                .SingleOrDefault();
        }

        public Task<WlnObject> GetObjectByUid(string uid, long flags = 1)
        {
            return GetObject(uid, PropertyElement.sys_unique_id, flags);
        }

        public Task<WlnObject[]> GetObjectsByProtocol(string mask, long flags = 1)
        {
            return GetObjects(mask, PropertyElement.rel_hw_type_name, flags);
        }

        public Task<WlnObject> GetObjectById(long id, long flags = 1)
        {
            return GetObject(id.ToString(), PropertyElement.sys_id, flags);
        }

        public Task<GroupObjects[]> GetGroupObjects(string mask = "*",
            PropertyElement propName = PropertyElement.sys_name, long flags = 1,
            uint force = 1, uint from = 0, uint to = 0)
        {
            var ss = new SearchSpector
            {
                ItemType = ItemType.GroupObjects,
                PropertyName = propName.ToString(),
                Mask = mask
            };

            return GetUnits<GroupObjects>(ss, force, flags, from, to);
        }

        public Task<Retranslator[]> GetRetranslators(string mask = "*",
            PropertyElement propName = PropertyElement.sys_name, long flags = 1,
            uint force = 1, uint from = 0, uint to = 0)
        {
            var ss = new SearchSpector
            {
                ItemType = ItemType.Retranslator,
                PropertyName = propName.ToString(),
                Mask = mask
            };

            return GetUnits<Retranslator>(ss, force, flags, from, to);
        }

        public Task<User[]> GetUsers(string mask = "*",
            PropertyElement propName = PropertyElement.sys_name, long flags = 1,
            uint force = 1, uint from = 0, uint to = 0)
        {
            var ss = new SearchSpector
            {
                ItemType = ItemType.User,
                PropertyName = propName.ToString(),
                Mask = mask
            };

            return GetUnits<User>(ss, force, flags, from, to);
        }

        public async Task<User> GetUser(string mask = "*",
            PropertyElement propName = PropertyElement.sys_name, long flags = 1,
            uint force = 1, uint from = 0, uint to = 0)
        {
            var ss = new SearchSpector
            {
                ItemType = ItemType.User,
                PropertyName = propName.ToString(),
                Mask = mask
            };

            return ( await GetUnits<User>(ss, force, flags, from, to) ).FirstOrDefault();
        }
    }

    public sealed class User : Unit
    {
        public Task<Dictionary<long, UnitAccess>> GetUnitsAccess(Session session, UnitAccessRequest.Params param)
        {
            return new UnitAccessRequest(session, param).GetResponse();
        }

        public Task<Dictionary<long, UnitAccess>> GetUnitsAccess(Session session, ItemType type = ItemType.Object)
        {
            return GetUnitsAccess
            (
                session,
                new UnitAccessRequest.Params
                {
                    UserId = Id,
                    ItemSuperclass = type
                }
            );
        }

        public async Task<WlnObject[]> GetObjects(Session session, long flags = 1)
        {
            var ss = new SearchService(session);

            var mask = (await GetUnitsAccess(session))
                ?.Keys?.Select(x => x.ToString()).Join(",");

            if(mask?.Length == 0) return null;

            return await ss.GetObjects(mask, PropertyElement.sys_id, flags);
        }
    }

    public sealed class UnitAccessRequest : Request<Dictionary<long, UnitAccess>>
    {
        public UnitAccessRequest(Session connection, Params parameters) : base(connection, parameters)
        {
        }

        public sealed class Params : RequestParams
        {
            [JsonProperty("userId")]
            public long UserId;

            [JsonProperty("directAccess")]
            [JsonConverter(typeof(NumberBoolConverter))]
            public bool DirectAccess;

            [JsonProperty("itemSuperclass")]
            [JsonConverter(typeof(StringEnumConverter))]
            public ItemType ItemSuperclass = ItemType.Object;

            [JsonProperty("flags")]
            public uint Flags = 0x1;
        }

        public override string Method => "user/get_items_access";
    }

    public sealed class UnitAccess
    {
        [JsonProperty("cacl")]
        public long Combine;

        [JsonProperty("dacl")]
        public long Direct;
    }
}