// @formatter:off
// ../Unity/Assets/Config/Excel/example.xlsx

using System.Collections.Generic;
using System.Collections.ObjectModel;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

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
    public partial class ExampleConfigCategory: ConfigSingleton<ExampleConfigCategory>
    {
        [BsonElement]
        [ProtoMember(1)]
        public IList<ExampleConfig> List = new List<ExampleConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, ExampleConfig> dict = new();

        public ExampleConfig Get(int id)
        {
            this.dict.TryGetValue(id, out ExampleConfig value);
            return value;
        }

        [ProtoAfterDeserialization]
        public void ProtoEndInit()
        {
            this.List = new ReadOnlyCollection<ExampleConfig>(this.List);

            foreach (var config in List)
            {
                config.AfterEndInit();
                this.dict.Add(config.ID, config);
            }

            this.AfterCategoryInit();
        }
    }
}