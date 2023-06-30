using System;

namespace ET
{
    public partial class KVCommon
    {
        public override void AfterEndInit()
        {
            if (this.Item_Coin <= 0)
            {
                throw new Exception($"KVCommon表Item_Coin配置错误");
            }

            if (this.Item_Diamond <= 0)
            {
                throw new Exception($"KVCommon表Item_Diamond配置错误");
            }

            if (this.Item_Exp <= 0)
            {
                throw new Exception($"KVCommon表Item_Exp配置错误");
            }
        }
    }
}