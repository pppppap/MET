using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class StartZoneConfigCategory: ConfigSingleton<StartZoneConfigCategory>, IMerge
    {
        [BsonElement]
        [ProtoMember(1)]
        private List<StartZoneConfig> list = new List<StartZoneConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, StartZoneConfig> dict = new();

        public StartZoneConfig Get(int id)
        {
            return this.dict[id];
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

        public List<StartZoneConfig> GetAll()
        {
            return this.list;
        }
    }

    [ProtoContract]
    public partial class StartZoneConfig: ProtoObject, IConfig
    {
        /// <summary>ID</summary>
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary>数据库地址</summary>
        [ProtoMember(2)]
        public string DBConnection { get; set; }

        /// <summary>数据库名</summary>
        [ProtoMember(3)]
        public string DBName { get; set; }
    }
}