// @formatter:off
// ../Unity/Assets/Config/Excel/StartConfig\Localhost\startconfig.xlsx

using System.Collections.Generic;
using System.Collections.ObjectModel;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    public partial class StartSceneConfig: ProtoObject, IConfig
    {
        /// <summary>ID</summary>
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary>所属进程</summary>
        [ProtoMember(2)]
        public int Process { get; set; }

        /// <summary>区</summary>
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

    [ProtoContract]
    [Config]
    public partial class StartSceneConfigCategory: ConfigSingleton<StartSceneConfigCategory>
    {
        [BsonElement]
        [ProtoMember(1)]
        public IList<StartSceneConfig> List { get; private set; } = new List<StartSceneConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, StartSceneConfig> dict = new();

        public StartSceneConfig Get(int id)
        {
            this.dict.TryGetValue(id, out StartSceneConfig value);
            return value;
        }

        [ProtoAfterDeserialization]
        public void ProtoEndInit()
        {
            this.List = new ReadOnlyCollection<StartSceneConfig>(this.List);

            foreach (var config in List)
            {
                config.AfterEndInit();
                this.dict.Add(config.ID, config);
            }

            this.AfterCategoryInit();
        }
    }
}