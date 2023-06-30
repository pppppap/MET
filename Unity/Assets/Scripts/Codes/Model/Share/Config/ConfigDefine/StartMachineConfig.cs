// @formatter:off
// ../Unity/Assets/Config/Excel/StartConfig\Localhost\startconfig.xlsx

using System.Collections.Generic;
using System.Collections.ObjectModel;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    public partial class StartMachineConfig: ProtoObject, IConfig
    {
        /// <summary>ID</summary>
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary>内网地址</summary>
        [ProtoMember(2)]
        public string InnerIP { get; set; }

        /// <summary>外网地址</summary>
        [ProtoMember(3)]
        public string OuterIP { get; set; }

        /// <summary>守护进程端口</summary>
        [ProtoMember(4)]
        public string WatcherPort { get; set; }
    }

    [ProtoContract]
    [Config]
    public partial class StartMachineConfigCategory: ConfigSingleton<StartMachineConfigCategory>
    {
        [BsonElement]
        [ProtoMember(1)]
        public IList<StartMachineConfig> List = new List<StartMachineConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, StartMachineConfig> dict = new();

        public StartMachineConfig Get(int id)
        {
            this.dict.TryGetValue(id, out StartMachineConfig value);
            return value;
        }

        [ProtoAfterDeserialization]
        public void ProtoEndInit()
        {
            this.List = new ReadOnlyCollection<StartMachineConfig>(this.List);

            foreach (var config in List)
            {
                config.AfterEndInit();
                this.dict.Add(config.ID, config);
            }

            this.AfterCategoryInit();
        }
    }
}