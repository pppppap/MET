// @formatter:off
// ../Unity/Assets/Config/Excel/sevenday.xlsx

using System.Collections.Generic;
using System.Collections.ObjectModel;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
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

    [ProtoContract]
    [Config]
    public partial class SevenDayConfigCategory: ConfigSingleton<SevenDayConfigCategory>
    {
        [BsonElement]
        [ProtoMember(1)]
        public IList<SevenDayConfig> List { get; private set; } = new List<SevenDayConfig>();

        [ProtoIgnore]
        [BsonIgnore]
        private readonly Dictionary<int, SevenDayConfig> dict = new();

        public SevenDayConfig Get(int id)
        {
            this.dict.TryGetValue(id, out SevenDayConfig value);
            return value;
        }

        [ProtoAfterDeserialization]
        public void ProtoEndInit()
        {
            this.List = new ReadOnlyCollection<SevenDayConfig>(this.List);

            foreach (var config in List)
            {
                config.AfterEndInit();
                this.dict.Add(config.ID, config);
            }

            this.AfterCategoryInit();
        }
    }
}