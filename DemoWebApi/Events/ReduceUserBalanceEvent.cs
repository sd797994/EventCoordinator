using EventCoordinator.ProcessManager.Interface;
using EventCoordinator.ProcessManager.Model;

namespace DemoWebApi.BusinessService.Dtos
{
    public class ReduceUserBalanceEvent : EventProcessBase
    {
        public string OrderNo { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }
    }
}
