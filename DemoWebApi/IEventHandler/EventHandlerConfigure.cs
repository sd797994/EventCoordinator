using System;
using System.Collections.Generic;
using System.Text;

namespace DemoWebApi.Common
{
    public struct EventHandlerConfigure
    {
        public struct ProcessManagerPipline
        {
            public const string OrderCreate = "OrderCreate";
        }
        public struct EventTopic
        {
            public const string PrepareCreateOrder = "PrepareCreateOrder";
            public const string ReduceProductStock = "ReduceProductStock";
            public const string RollBackProductStock = "RollBackProductStock";
            public const string ReduceUserBalance = "ReduceUserBalance";
            public const string RollBackUserBalance = "RollBackUserBalance";
            public const string CreateOrder = "CreateOrder";
        }
    }
}
