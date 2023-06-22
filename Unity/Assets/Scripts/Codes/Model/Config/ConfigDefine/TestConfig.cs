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
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, TestConfig> dict = new Dictionary<int, TestConfig>();

        [BsonElement]
        [ProtoMember(1)]
        private List<TestConfig> list = new List<TestConfig>();

        public void Merge(object o)
        {
            TestConfigCategory s = o as TestConfigCategory;
            this.list.AddRange(s.list);
        }

        [ProtoAfterDeserialization]
        public void ProtoEndInit()
        {
            foreach (TestConfig config in list)
            {
                config.AfterEndInit();
                this.dict.Add(config.ID, config);
            }

            this.list.Clear();

            this.AfterEndInit();
        }

        public TestConfig Get(int id)
        {
            this.dict.TryGetValue(id, out TestConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (TestConfig)}，配置id: {id}");
            }

            return item;
        }

        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, TestConfig> GetAll()
        {
            return this.dict;
        }

        public TestConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }

            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
    public partial class TestConfig: ProtoObject, IConfig
    {
        /// <summary>唯一id</summary>
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary>所属ai</summary>
        [ProtoMember(2)]
        public int AIConfigId { get; set; }

        /// <summary>此ai中的顺序</summary>
        [ProtoMember(3)]
        public int Order { get; set; }

        /// <summary>名字</summary>
        [ProtoMember(4)]
        public string Name { get; set; }

        /// <summary>节点参数</summary>
        [ProtoMember(5)]
        public int[] NodeParams { get; set; }
    }
}