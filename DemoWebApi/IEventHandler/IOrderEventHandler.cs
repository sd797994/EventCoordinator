using DemoWebApi.BusinessService.Dtos;
using EventCoordinator.ProcessManager.Interface;
using EventCoordinator.ProcessManager.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebApi.BusinessService.Interface
{
    public interface IOrderEventHandler
    {
        Task PrepareCreateOrder(PrepareCreateOrderEvent input);
        Task CreateOrder(CreateOrderEvent input);
    }
}
