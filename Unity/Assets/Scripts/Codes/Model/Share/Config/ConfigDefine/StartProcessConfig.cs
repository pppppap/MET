// @formatter:off
// ../Unity/Assets/Config/Excel/StartConfig\Localhost\startconfig.xlsx

using System.Collections.Generic;
using System.Collections.ObjectModel;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

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
    public partial class StartProcessConfigCategory: ConfigSingleton<StartProcessConfigCategory>
    {
        [BsonElement]
        [ProtoMember(1)]
        public IList<StartProcessConfig> List = new List<StartProcessConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, StartProcessConfig> dict = new();

        public StartProcessConfig Get(int id)
        {
            this.dict.TryGetValue(id, out StartProcessConfig value);
            return value;
        }

        [ProtoAfterDeserialization]
        public void ProtoEndInit()
        {
            this.List = new ReadOnlyCollection<StartProcessConfig>(this.List);

            foreach (var config in List)
            {
                config.AfterEndInit();
                this.dict.Add(config.ID, config);
            }

            this.AfterCategoryInit();
        }
    }
}