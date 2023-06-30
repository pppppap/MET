using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

// ../Unity/Assets/Config/Excel/kv.xlsx

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
    public partial class KVCommonCategory: ConfigSingleton<KVCommonCategory>, IMerge
    {
        [BsonElement]
        [ProtoMember(1)]
        private List<KVCommon> list = new List<KVCommon>();

        public KVCommon Get()
        {
            return list[0];
        }

        public void Merge(object o)
        {
            KVCommonCategory s = o as KVCommonCategory;
            this.list.AddRange(s.list);
        }

        [ProtoAfterDeserialization]
        public void ProtoEndInit()
        {
            foreach (var config in list)
            {
                config.AfterEndInit();
            }

            this.AfterEndInit();
        }
    }
}