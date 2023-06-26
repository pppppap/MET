namespace ET
{
    public partial class SevenDayConfigCategory
    {
        public int MaxDay;

        public override void AfterEndInit()
        {
            foreach (SevenDayConfig sevenDayConfig in this.list)
            {
                if (this.MaxDay < sevenDayConfig.ID)
                {
                    this.MaxDay = sevenDayConfig.ID;
                }
            }
        }
    }
}