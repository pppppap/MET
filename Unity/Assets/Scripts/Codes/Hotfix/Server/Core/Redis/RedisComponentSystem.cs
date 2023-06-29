using StackExchange.Redis;

namespace ET.Server
{
    [FriendOf(typeof(RedisComponent))]
    public static class RedisComponentSystem
    {
        [ObjectSystem]
        public class RedisComponentAwakeSystem: AwakeSystem<RedisComponent, int>
        {
            protected override void Awake(RedisComponent self, int zone)
            {
                self.Zone = zone;
                self.RedisDB = self.ZoneConfig.RedisDB;
                self.Options = ConfigurationOptions.Parse(self.ZoneConfig.RedisAddress);
            }
        }
    }
}