using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

// ../Unity/Assets/Config/Excel/StartConfig\Localhost\startconfig.xlsx

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class StartMachineConfigCategory: ConfigSingleton<StartMachineConfigCategory>, IMerge
    {
        [BsonElement]
        [ProtoMember(1)]
        private List<StartMachineConfig> list = new List<StartMachineConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, StartMachineConfig> dict = new();

        public StartMachineConfig Get(int id)
        {
            this.dict.TryGetValue(id, out StartMachineConfig value);
            return value;
        }

        public void Merge(object o)
        {
            StartMachineConfigCategory s = o as StartMachineConfigCategory;
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

        public List<StartMachineConfig> GetAll()
        {
            return this.list;
        }
    }

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
}