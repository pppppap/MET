using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

// ../Unity/Assets/Config/Excel/StartConfig\Localhost\startconfig.xlsx

namespace ET
{
    [ProtoContract]
    public partial class StartProcessConfig: ProtoObject, IConfig
    {
        /// <summary>ID</summary>
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary>所属机器</summary>
        [ProtoMember(2)]
        public int MachineId { get; set; }

        /// <summary>内网端口</summary>
        [ProtoMember(3)]
        public int InnerPort { get; set; }
    }

    [ProtoContract]
    [Config]
    public partial class StartProcessConfigCategory: ConfigSingleton<StartProcessConfigCategory>, IMerge
    {
        [BsonElement]
        [ProtoMember(1)]
        private List<StartProcessConfig> list = new List<StartProcessConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        public List<StartProcessConfig> List => this.list;

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, StartProcessConfig> dict = new();

        public StartProcessConfig Get(int id)
        {
            this.dict.TryGetValue(id, out StartProcessConfig value);
            return value;
        }

        public void Merge(object o)
        {
            StartProcessConfigCategory s = o as StartProcessConfigCategory;
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