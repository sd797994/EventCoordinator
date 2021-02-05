using DemoWebApi.BusinessService.Dtos;
using DemoWebApi.BusinessService.Interface;
using DemoWebApi.Common;
using EventCoordinator.Common;
using EventCoordinator.Event.Model;
using EventCoordinator.ProcessManager.Impl;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebApi.BusinessService.Impl
{
    public class OrderEventHandler : EventProcessHandlerBase, IOrderEventHandler
    {
        private readonly ILogger<OrderEventHandler> logger;
        public OrderEventHandler(ILogger<OrderEventHandler> logger)
        {
            this.logger = logger;
        }
        [EventHandler(EventHandlerConfigure.EventTopic.CreateOrder)]
        public async Task CreateOrder(CreateOrderEvent input)
        {
            //随机模拟50%概率回退
            if (RandomHelper.GetRandomBool())
            {
                logger.LogInformation("订单创建成功!");
                await Task.CompletedTask;
            }
            else
            {
                logger.LogWarning("订单创建失败!");
                await RollBack();
            }
        }
        [EventHandler(EventHandlerConfigure.EventTopic.PrepareCreateOrder)]
        public async Task PrepareCreateOrder(PrepareCreateOrderEvent input)
        {
            var data = new ReduceProductStockEvent();
            data.OrderNo = $"{DateTime.Now:YYYYMMDDHHmmss}{RandomHelper.GetRandomInt(10000, 99999)}";
            data.UserId = 1;
            data.Items = new List<ReduceProductStockEvent.ReduceProductStockItem>();
            input.produceItems.ForEach(x => data.Items.Add(new ReduceProductStockEvent.ReduceProductStockItem() { ProductId = x.ProductId, Count = x.Count }));
            logger.LogInformation($"预处理订单创建成功,订单编号:{data.OrderNo}!");
            await CallNext(data);
        }
    }
}
