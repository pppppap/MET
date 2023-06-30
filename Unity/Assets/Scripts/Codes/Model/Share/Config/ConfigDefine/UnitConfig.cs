using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

// ../Unity/Assets/Config/Excel/unit.xlsx

namespace ET
{
    [ProtoContract]
    public partial class UnitConfig: ProtoObject, IConfig
    {
        /// <summary>ID</summary>
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary>Type</summary>
        [ProtoMember(2)]
        public int Type { get; set; }

        /// <summary>名字</summary>
        [ProtoMember(3)]
        public string Name { get; set; }

        /// <summary>描述</summary>
        [ProtoMember(4)]
        public string Desc { get; set; }

        /// <summary>位置</summary>
        [ProtoMember(5)]
        public int Position { get; set; }

        /// <summary>身高</summary>
        [ProtoMember(6)]
        public int Height { get; set; }

        /// <summary>体重</summary>
        [ProtoMember(7)]
        public int Weight { get; set; }
    }

    [ProtoContract]
    [Config]
    public partial class UnitConfigCategory: ConfigSingleton<UnitConfigCategory>, IMerge
    {
        [BsonElement]
        [ProtoMember(1)]
        private List<UnitConfig> list = new List<UnitConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        public List<UnitConfig> List => this.list;

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, UnitConfig> dict = new();

        public UnitConfig Get(int id)
        {
            this.dict.TryGetValue(id, out UnitConfig value);
            return value;
        }

        public void Merge(object o)
        {
            UnitConfigCategory s = o as UnitConfigCategory;
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