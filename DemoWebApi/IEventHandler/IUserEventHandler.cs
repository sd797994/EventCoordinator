using DemoWebApi.BusinessService.Dtos;
using EventCoordinator.ProcessManager.Interface;
using EventCoordinator.ProcessManager.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebApi.BusinessService.Interface
{
    public interface IUserEventHandler
    {
        Task ReduceUserBalance(ReduceUserBalanceEvent input);
        Task RollBackUserBalance(ReduceUserBalanceEvent input);
    }
}
