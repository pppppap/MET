// @formatter:off
// ../Unity/Assets/Config/Excel/StartConfig\Localhost\startconfig.xlsx

using System.Collections.Generic;
using System.Collections.ObjectModel;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

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
    public partial class StartZoneConfigCategory: ConfigSingleton<StartZoneConfigCategory>
    {
        [BsonElement]
        [ProtoMember(1)]
        public IList<StartZoneConfig> List = new List<StartZoneConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, StartZoneConfig> dict = new();

        public StartZoneConfig Get(int id)
        {
            this.dict.TryGetValue(id, out StartZoneConfig value);
            return value;
        }

        [ProtoAfterDeserialization]
        public void ProtoEndInit()
        {
            this.List = new ReadOnlyCollection<StartZoneConfig>(this.List);

            foreach (var config in List)
            {
                config.AfterEndInit();
                this.dict.Add(config.ID, config);
            }

            this.AfterCategoryInit();
        }
    }
}