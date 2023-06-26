using ET.EventType;

namespace ET.Server
{
    public static class SevenDayComponentSystem
    {
        [Event(SceneType.Map)]
        [FriendOf(typeof (SevenDayComponent))]
        public class DayReset_SevenDay: AEvent<DayReset>
        {
            protected override async ETTask Run(Scene scene, DayReset e)
            {
                Unit unit = e.Unit;

                SevenDayComponent sevenDayComponent = unit.GetComponent<SevenDayComponent>();
                if (sevenDayComponent == null)
                {
                    return;
                }

                if (sevenDayComponent.CurDay >= SevenDayConfigCategory.Instance.MaxDay)
                {
                    return;
                }

                sevenDayComponent.CurDay++;
                await ETTask.CompletedTask;
            }
        }

        public static void ReceiveReward(this SevenDayComponent self, int day)
        {
            bool canReceive = self.CanReceive(day);
            if (!canReceive)
            {
                throw new LogicException(1);
            }

            self.GetDayRecord(day);
        }
    }
}