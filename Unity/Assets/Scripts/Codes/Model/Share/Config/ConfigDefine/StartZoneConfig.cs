using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

// ../Unity/Assets/Config/Excel/StartConfig\Localhost\startconfig.xlsx

namespace ET
{
    [ProtoContract]
    public partial class StartZoneConfig: ProtoObject, IConfig
    {
        /// <summary>ID</summary>
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary>数据库地址</summary>
        [ProtoMember(2)]
        public string DBAddress { get; set; }

        /// <summary>数据库名</summary>
        [ProtoMember(3)]
        public string DBName { get; set; }

        /// <summary>Redis地址</summary>
        [ProtoMember(4)]
        public string RedisAddress { get; set; }

        /// <summary>Redis数据库</summary>
        [ProtoMember(5)]
        public int RedisDB { get; set; }
    }

    [ProtoContract]
    [Config]
    public partial class StartZoneConfigCategory: ConfigSingleton<StartZoneConfigCategory>, IMerge
    {
        [BsonElement]
        [ProtoMember(1)]
        private List<StartZoneConfig> list = new List<StartZoneConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        public List<StartZoneConfig> List => this.list;

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, StartZoneConfig> dict = new();

        public StartZoneConfig Get(int id)
        {
            this.dict.TryGetValue(id, out StartZoneConfig value);
            return value;
        }

        public void Merge(object o)
        {
            StartZoneConfigCategory s = o as StartZoneConfigCategory;
            this.list.AddRange(s.list);
        }

        [ProtoAfterDeserialization]
        public void ProtoEndInit()
        {
            foreach (var config in list)
            {
                config.AfterEndInit();
                this.dict.Add(config.ID, config);
            }

            this.AfterEndInit();
        }
    }
}