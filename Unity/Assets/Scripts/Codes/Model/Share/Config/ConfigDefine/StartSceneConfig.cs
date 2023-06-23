using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class StartSceneConfigCategory: ConfigSingleton<StartSceneConfigCategory>, IMerge
    {
        [BsonElement]
        [ProtoMember(1)]
        private List<StartSceneConfig> list = new List<StartSceneConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, StartSceneConfig> dict = new();

        public StartSceneConfig Get(int id)
        {
            this.dict.TryGetValue(id, out StartSceneConfig value);
            return value;
        }

        public void Merge(object o)
        {
            StartSceneConfigCategory s = o as StartSceneConfigCategory;
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

        public List<StartSceneConfig> GetAll()
        {
            return this.list;
        }
    }

    [ProtoContract]
    public partial class StartSceneConfig: ProtoObject, IConfig
    {
        /// <summary>ID</summary>
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary>所属进程</summary>
        [ProtoMember(2)]
        public int Process { get; set; }

        /// <summary>所属区</summary>
        [ProtoMember(3)]
        public int Zone { get; set; }

        /// <summary>类型</summary>
        [ProtoMember(4)]
        public string SceneType { get; set; }

        /// <summary>名字</summary>
        [ProtoMember(5)]
        public string Name { get; set; }

        /// <summary>外网端口</summary>
        [ProtoMember(6)]
        public int OuterPort { get; set; }
    }
}