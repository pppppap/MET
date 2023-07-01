using MongoDB.Driver;
using StackExchange.Redis;

namespace ET.Server
{
    /// <summary>
    /// 用来缓存数据
    /// </summary>
    [ChildOf(typeof(DBManagerComponent))]
    public class DBComponent: Entity, IAwake<ZoneConfig>, IDestroy
    {
        public const int TaskCount = 32;
        public int zone;

        public MongoClient mongoClient;
        public IMongoDatabase database;

        public ZoneConfig Config => ZoneConfigCategory.Instance.Get(this.zone);
        public ConnectionMultiplexer RedisConnection;
        public IDatabase RedisDB;
    }
}