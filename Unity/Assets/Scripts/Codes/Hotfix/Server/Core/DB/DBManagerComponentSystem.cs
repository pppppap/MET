using System;

namespace ET.Server
{
    [FriendOf(typeof (DBManagerComponent))]
    public static class DBManagerComponentSystem
    {
        [ObjectSystem]
        public class DBManagerComponentAwakeSystem: AwakeSystem<DBManagerComponent>
        {
            protected override void Awake(DBManagerComponent self)
            {
                DBManagerComponent.Instance = self;
            }
        }

        [ObjectSystem]
        public class DBManagerComponentDestroySystem: DestroySystem<DBManagerComponent>
        {
            protected override void Destroy(DBManagerComponent self)
            {
                DBManagerComponent.Instance = null;
            }
        }

        public static DBComponent GetZoneDB(this DBManagerComponent self, int zone)
        {
            DBComponent dbComponent = self.DBComponents[zone];
            if (dbComponent != null)
            {
                return dbComponent;
            }

            ZoneConfig zoneConfig = ZoneConfigCategory.Instance.Get(zone);

            if (zoneConfig.MongoHost == "" || zoneConfig.MongoPort == 0)
            {
                throw new Exception($"zone: {zone} not found mongo connect string");
            }

            dbComponent = self.AddChild<DBComponent, ZoneConfig>(zoneConfig);
            self.DBComponents[zone] = dbComponent;
            return dbComponent;
        }
    }
}