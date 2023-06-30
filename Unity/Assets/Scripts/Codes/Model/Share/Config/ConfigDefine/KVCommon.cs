// @formatter:off
// ../Unity/Assets/Config/Excel/kv.xlsx

using System.Collections.Generic;
using System.Collections.ObjectModel;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    public partial class KVCommon: ProtoObject, IConfig
    {
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary>经验道具ID</summary>
        [ProtoMember(2)]
        public int Item_Exp { get; set; }

        /// <summary>货币道具ID</summary>
        [ProtoMember(3)]
        public int Item_Coin { get; set; }

        /// <summary>钻石道具ID</summary>
        [ProtoMember(4)]
        public int Item_Diamond { get; set; }
    }

    [ProtoContract]
    [Config]
    public partial class KVCommonCategory: ConfigSingleton<KVCommonCategory>
    {
        [BsonElement]
        [ProtoMember(1)]
        public IList<KVCommon> List = new List<KVCommon>();

        public KVCommon Get()
        {
            return List[0];
        }

        [ProtoAfterDeserialization]
        public void ProtoEndInit()
        {
            this.List = new ReadOnlyCollection<KVCommon>(this.List);

            foreach (var config in List)
            {
                config.AfterEndInit();
            }

            this.AfterEndInit();
        }
    }
}