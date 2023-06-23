using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
	public partial class AIConfigCategory
	{
		[ProtoIgnore]
		[BsonIgnore]
		public Dictionary<int, SortedDictionary<int, AIConfig>> AIConfigs = new Dictionary<int, SortedDictionary<int, AIConfig>>();

		public SortedDictionary<int, AIConfig> GetAI(int aiConfigId)
		{
			return this.AIConfigs[aiConfigId];
		}

		public override void AfterEndInit()
		{
			foreach (var cfg in this.GetAll())
			{
				SortedDictionary<int, AIConfig> aiNodeConfig;
				if (!this.AIConfigs.TryGetValue(cfg.AIConfigId, out aiNodeConfig))
				{
					aiNodeConfig = new SortedDictionary<int, AIConfig>();
					this.AIConfigs.Add(cfg.AIConfigId, aiNodeConfig);
				}
				
				aiNodeConfig.Add(cfg.ID, cfg);
			}
		}
	}
}
