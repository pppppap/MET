using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class TestConfigCategory: ConfigSingleton<TestConfigCategory>, IMerge
    {
        [BsonElement]
        [ProtoMember(1)]
        private List<TestConfig> list = new List<TestConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, TestConfig> dict = new();

        public TestConfig Get(int id)
        {
            return this.dict[id];
        }

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, TestConfig> dictByType = new();

        public TestConfig GetByType(int Type)
        {
            return this.dictByType[Type];
        }

        public void Merge(object o)
        {
            TestConfigCategory s = o as TestConfigCategory;
            this.list.AddRange(s.list);
        }

        [ProtoAfterDeserialization]
        public void ProtoEndInit()
        {
            foreach (var config in list)
            {
                config.AfterEndInit();
                this.dict.Add(config.ID, config);
                this.dictByType.Add(config.Type, config);
            }

            this.AfterEndInit();
        }

        public List<TestConfig> GetAll()
        {
            return this.list;
        }
    }

    [ProtoContract]
    public partial class TestConfig: ProtoObject, IConfig
    {
        /// <summary>唯一id</summary>
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary>类型</summary>
        [ProtoMember(2)]
        public int Type { get; set; }

        /// <summary>所属ai</summary>
        [ProtoMember(3)]
        public int AIConfigId { get; set; }

        /// <summary>此ai中的顺序</summary>
        [ProtoMember(4)]
        public int Order { get; set; }

        /// <summary>名字</summary>
        [ProtoMember(5)]
        public string Name { get; set; }

        /// <summary>节点参数</summary>
        [ProtoMember(6)]
        public int[] NodeParams { get; set; }
    }
}