using EventCoordinator.ProcessManager.Interface;
using System.Collections.Generic;

namespace DemoWebApi.BusinessService.Dtos
{
    public class PrepareCreateOrderEvent : EventProcessBase
    {
        public List<BuyProduceItem> produceItems { get; set; }
        public class BuyProduceItem
        {
            public int ProductId { get; set; }
            public int Count { get; set; }
        }
    }
}
