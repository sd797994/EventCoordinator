using DemoWebApi.BusinessService.Dtos;
using DemoWebApi.BusinessService.Interface;
using DemoWebApi.Common;
using EventCoordinator.ProcessManager.Interface;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DemoWebApi
{
    public class MyHostService : IHostedService
    {
        private readonly IProcessManager processManager;
        private readonly IOrderEventHandler orderEventHandler;
        private readonly IProductEventHandler productEventHandler;
        private readonly IUserEventHandler userEventHandler;
        public MyHostService(IProcessManager processManager,
            IOrderEventHandler orderEventHandler,
            IProductEventHandler productEventHandler,
            IUserEventHandler userEventHandler
            )
        {
            this.processManager = processManager;
            this.orderEventHandler = orderEventHandler;
            this.productEventHandler = productEventHandler;
            this.userEventHandler = userEventHandler;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            ProcessPiplineRegister(processManager);
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
        public void ProcessPiplineRegister(IProcessManager processManager)
        {
            //注册订单创建流程
            processManager.ResgiterProcess(EventHandlerConfigure.ProcessManagerPipline.OrderCreate)//注册创建订单流程名
                .ResgiterPipline<IOrderEventHandler, PrepareCreateOrderEvent>(orderEventHandler.PrepareCreateOrder)//注册预创建订单
                .ResgiterPipline<IProductEventHandler, ReduceProductStockEvent>(productEventHandler.ReduceProductStock, productEventHandler.RollBackProductStock)//注册产品减库存和补偿事件
                .ResgiterPipline<IUserEventHandler, ReduceUserBalanceEvent>(userEventHandler.ReduceUserBalance, userEventHandler.RollBackUserBalance)//注册用户扣余额和补偿事件
                .ResgiterPipline<IOrderEventHandler, CreateOrderEvent>(orderEventHandler.CreateOrder)//注册订单创建事件
                .BuildProcess();
        }
    }
}
