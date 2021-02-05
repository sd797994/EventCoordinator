using EventCoordinator.ProcessManager.Interface;

namespace DemoWebApi.BusinessService.Dtos
{
    public class CreateOrderEvent : EventProcessBase
    {
        public string OrderNo { get; set; }
    }
}
