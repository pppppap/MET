using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

// ../Unity/Assets/Config/Excel/ai.xlsx

namespace ET
{
    [ProtoContract]
    public partial class AIConfig: ProtoObject, IConfig
    {
        /// <summary>ID</summary>
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary>所属ai</summary>
        [ProtoMember(2)]
        public int AIConfigId { get; set; }

        /// <summary>此ai中的顺序</summary>
        [ProtoMember(3)]
        public int Order { get; set; }

        /// <summary>节点名字</summary>
        [ProtoMember(4)]
        public string Name { get; set; }

        /// <summary>描述</summary>
        [ProtoMember(5)]
        public string Desc { get; set; }

        /// <summary>节点参数</summary>
        [ProtoMember(6)]
        public int[] NodeParams { get; set; }
    }

    [ProtoContract]
    [Config]
    public partial class AIConfigCategory: ConfigSingleton<AIConfigCategory>, IMerge
    {
        [BsonElement]
        [ProtoMember(1)]
        private List<AIConfig> list = new List<AIConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        public List<AIConfig> List => this.list;

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, AIConfig> dict = new();

        public AIConfig Get(int id)
        {
            this.dict.TryGetValue(id, out AIConfig value);
            return value;
        }

        public void Merge(object o)
        {
            AIConfigCategory s = o as AIConfigCategory;
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