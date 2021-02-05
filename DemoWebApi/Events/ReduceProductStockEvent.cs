using EventCoordinator.ProcessManager.Interface;
using System.Collections.Generic;

namespace DemoWebApi.BusinessService.Dtos
{
    public class ReduceProductStockEvent : EventProcessBase
    {
        public int UserId { get; set; }
        public string OrderNo { get; set; }
        public List<ReduceProductStockItem> Items { get; set; }
        public class ReduceProductStockItem
        {
            public int ProductId { get; set; }
            public int Count { get; set; }
        }
    }
}
