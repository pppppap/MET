using ET.EventType;

namespace ET
{
    public static class UnitSystem
    {
        [ObjectSystem]
        public class UnitUpdateSystem: UpdateSystem<Unit>
        {
            protected override void Update(Unit self)
            {
                if (self.Type == UnitType.Player)
                {
                    self.UpdateOnlineTime();
                }
            }
        }

        [ObjectSystem]
        public class UnitAwakeSystem: AwakeSystem<Unit, int>
        {
            protected override void Awake(Unit self, int configId)
            {
                self.ConfigId = configId;
            }
        }

        public static void UpdateOnlineTime(this Unit self)
        {
            var nowTicks = TimeHelper.ServerNow();
            var now = TimeHelper.ToDateTime(nowTicks);
            var lastOnlineTime = TimeHelper.ToDateTime(self.LastOnlineTime);
            if (lastOnlineTime >= now)
            {
                return;
            }

            self.LastOnlineTime = nowTicks;

            if (now.Year != lastOnlineTime.Year || now.Month != lastOnlineTime.Month || now.Day != lastOnlineTime.Day)
            {
                EventSystem.Instance.Publish(self.DomainScene(), new DayReset { Unit = self });
            }
        }
    }
}