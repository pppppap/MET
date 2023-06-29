using StackExchange.Redis;

namespace ET.Server
{
    public static class RedisComponentSystem
    {
        [ObjectSystem]
        public class RedisComponentAwakeSystem: AwakeSystem<RedisComponent>
        {
            protected override void Awake(RedisComponent self)
            {
                self.Options = ConfigurationOptions.Parse("100.64.254.231:6379");
            }
        }
    }
}