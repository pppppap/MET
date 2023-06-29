using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

// ../Unity/Assets/Config/Excel/sevenday.xlsx

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class SevenDayConfigCategory: ConfigSingleton<SevenDayConfigCategory>, IMerge
    {
        [BsonElement]
        [ProtoMember(1)]
        private List<SevenDayConfig> list = new List<SevenDayConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, SevenDayConfig> dict = new();

        public SevenDayConfig Get(int id)
        {
            this.dict.TryGetValue(id, out SevenDayConfig value);
            return value;
        }

        public void Merge(object o)
        {
            SevenDayConfigCategory s = o as SevenDayConfigCategory;
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

        public List<SevenDayConfig> GetAll()
        {
            return this.list;
        }
    }

    [ProtoContract]
    public partial class SevenDayConfig: ProtoObject, IConfig
    {
        /// <summary>唯一id</summary>
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary>道具</summary>
        [ProtoMember(2)]
        public int[] Items { get; set; }
    }
}