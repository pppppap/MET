using StackExchange.Redis;

namespace ET.Server
{
    [ComponentOf(typeof (Scene))]
    public class RedisComponent: Entity, IAwake
    {
        public ConfigurationOptions Options;
    }
}