namespace ET
{
    public partial class SevenDayConfigCategory
    {
        public int MaxDay;

        public override void AfterCategoryInit()
        {
            foreach (SevenDayConfig sevenDayConfig in this.List)
            {
                if (this.MaxDay < sevenDayConfig.ID)
                {
                    this.MaxDay = sevenDayConfig.ID;
                }
            }
        }
    }
}