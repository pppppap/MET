using System;

namespace ET
{
    public class LogicException: Exception
    {
        public int ErrorCode;

        public LogicException(int code)
        {
            this.ErrorCode = code;
        }
    }
}