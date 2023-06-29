using StackExchange.Redis;

namespace ET.Server
{
    [ComponentOf(typeof (Scene))]
    public class RedisComponent: Entity, IAwake<int>
    {
        public ConfigurationOptions Options;
        public int Zone;
        public int RedisDB;
        public StartZoneConfig ZoneConfig => StartZoneConfigCategory.Instance.Get(this.Zone);

        private ConnectionMultiplexer _connection;

        public ConnectionMultiplexer Connection
        {
            get
            {
                if (this._connection == null)
                {
                    _connection = ConnectionMultiplexer.Connect(this.Options);
                }

                return this._connection;
            }
        }

        public IDatabase DB => Connection.GetDatabase(RedisDB);
    }
}