using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
    [ComponentOf(typeof (Unit))]
    public class SevenDayComponent: Entity
    {
        public int CurDay;
        public List<DayRecord> Records = new();
    }

    public class DayRecord
    {
        public int Day;
        public bool Recevied;
        public List<int[]> Items = new();

        [BsonIgnore]
        public SevenDayConfig Config
        {
            get
            {
                return SevenDayConfigCategory.Instance.Get(this.Day);
            }
        }
    }

    [FriendOf(typeof (SevenDayComponent))]
    public static class SevenDayComponentSystem
    {
        public static DayRecord GetDayRecord(this SevenDayComponent self, int day)
        {
            foreach (DayRecord r in self.Records)
            {
                if (r.Day == day)
                {
                    return r;
                }
            }

            return null;
        }

        public static bool CanReceive(this SevenDayComponent self, int day)
        {
            return self.GetDayStaus(day) == RewardStatus.CanReceive;
        }

        public static RewardStatus GetDayStaus(this SevenDayComponent self, int day)
        {
            DayRecord record = self.GetDayRecord(day);
            if (record == null)
            {
                return RewardStatus.CannotReceive;
            }

            if (record.Recevied)
            {
                return RewardStatus.Received;
            }

            if (self.CurDay < day)
            {
                return RewardStatus.CannotReceive;
            }

            return RewardStatus.CanReceive;
        }
    }
}