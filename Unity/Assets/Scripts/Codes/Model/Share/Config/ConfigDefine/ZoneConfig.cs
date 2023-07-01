// @formatter:off
// ../Unity/Assets/Config/Excel/StartConfig\Localhost\startconfig.xlsx

using System.Collections.Generic;
using System.Collections.ObjectModel;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    public partial class ZoneConfig: ProtoObject, IConfig
    {
        /// <summary>唯一id</summary>
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary></summary>
        [ProtoMember(2)]
        public string MongoHost { get; set; }

        /// <summary></summary>
        [ProtoMember(3)]
        public int MongoPort { get; set; }

        /// <summary></summary>
        [ProtoMember(4)]
        public string MongoDBName { get; set; }

        /// <summary></summary>
        [ProtoMember(5)]
        public string RedisHost { get; set; }

        /// <summary></summary>
        [ProtoMember(6)]
        public int RedisPort { get; set; }

        /// <summary></summary>
        [ProtoMember(7)]
        public int RedisDBIndex { get; set; }

        /// <summary></summary>
        [ProtoMember(8)]
        public string RedisPassword { get; set; }
    }

    [ProtoContract]
    [Config]
    public partial class ZoneConfigCategory: ConfigSingleton<ZoneConfigCategory>
    {
        [BsonElement]
        [ProtoMember(1)]
        public IList<ZoneConfig> List { get; private set; } = new List<ZoneConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, ZoneConfig> dict = new();

        public ZoneConfig Get(int id)
        {
            this.dict.TryGetValue(id, out ZoneConfig value);
            return value;
        }

        [ProtoAfterDeserialization]
        public void ProtoEndInit()
        {
            this.List = new ReadOnlyCollection<ZoneConfig>(this.List);

            foreach (var config in List)
            {
                config.AfterEndInit();
                this.dict.Add(config.ID, config);
            }

            this.AfterCategoryInit();
        }
    }
}