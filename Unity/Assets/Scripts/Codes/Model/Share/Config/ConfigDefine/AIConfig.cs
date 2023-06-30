// @formatter:off
// ../Unity/Assets/Config/Excel/ai.xlsx

using System.Collections.Generic;
using System.Collections.ObjectModel;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

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
    public partial class AIConfigCategory: ConfigSingleton<AIConfigCategory>
    {
        [BsonElement]
        [ProtoMember(1)]
        public IList<AIConfig> List { get; private set; } = new List<AIConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, AIConfig> dict = new();

        public AIConfig Get(int id)
        {
            this.dict.TryGetValue(id, out AIConfig value);
            return value;
        }

        [ProtoAfterDeserialization]
        public void ProtoEndInit()
        {
            this.List = new ReadOnlyCollection<AIConfig>(this.List);

            foreach (var config in List)
            {
                config.AfterEndInit();
                this.dict.Add(config.ID, config);
            }

            this.AfterCategoryInit();
        }
    }
}