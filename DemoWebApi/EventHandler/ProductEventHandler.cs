using DemoWebApi.BusinessService.Dtos;
using DemoWebApi.BusinessService.Interface;
using DemoWebApi.Common;
using EventCoordinator.Common;
using EventCoordinator.Event.Model;
using EventCoordinator.ProcessManager.Impl;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebApi.BusinessService.Impl
{
    public class ProductEventHandler : EventProcessHandlerBase, IProductEventHandler
    {
        private readonly ILogger<ProductEventHandler> logger;
        public ProductEventHandler(ILogger<ProductEventHandler> logger)
        {
            this.logger = logger;
        }
        [EventHandler(EventHandlerConfigure.EventTopic.ReduceProductStock)]
        public async Task ReduceProductStock(ReduceProductStockEvent input)
        {
            //随机模拟50%概率回退
            if (RandomHelper.GetRandomBool())
            {
                var data = new ReduceUserBalanceEvent();
                data.OrderNo = input.OrderNo;
                data.UserId = 1;
                data.Balance = input.Items.Sum(x => x.Count * (decimal)2.88);
                logger.LogInformation($"商品预扣库存成功,订单编号:{input.OrderNo},商品数量:{input.Items.Sum(x => x.Count)}!");
                await CallNext(data);
            }
            else
            {
                logger.LogWarning("商品预扣库存失败!");
                await RollBack();
            }
        }

        [EventHandler(EventHandlerConfigure.EventTopic.RollBackProductStock)]
        public async Task RollBackProductStock(ReduceProductStockEvent input)
        {
            //随机模拟50%概率补偿失败
            if (RandomHelper.GetRandomBool())
            {
                logger.LogInformation($"商品库存回滚成功,订单编号:{input.OrderNo},商品数量:{input.Items.Sum(x => x.Count)}!");
            }
            else
            {
                logger.LogError($"商品库存回滚异常!!");
                await RollBack();
            }
        }
    }
}
