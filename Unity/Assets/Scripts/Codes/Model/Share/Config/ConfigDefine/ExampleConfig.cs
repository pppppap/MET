using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

// ../Unity/Assets/Config/Excel/example.xlsx

namespace ET
{
    [ProtoContract]
    public partial class ExampleConfig: ProtoObject, IConfig
    {
        /// <summary>唯一id</summary>
        [ProtoMember(1)]
        public int ID { get; set; }
    }

    [ProtoContract]
    [Config]
    public partial class ExampleConfigCategory: ConfigSingleton<ExampleConfigCategory>, IMerge
    {
        [BsonElement]
        [ProtoMember(1)]
        private List<ExampleConfig> list = new List<ExampleConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        public List<ExampleConfig> List => this.list;

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, ExampleConfig> dict = new();

        public ExampleConfig Get(int id)
        {
            this.dict.TryGetValue(id, out ExampleConfig value);
            return value;
        }

        public void Merge(object o)
        {
            ExampleConfigCategory s = o as ExampleConfigCategory;
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