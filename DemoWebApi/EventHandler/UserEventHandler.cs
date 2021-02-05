using DemoWebApi.BusinessService.Dtos;
using DemoWebApi.BusinessService.Interface;
using DemoWebApi.Common;
using EventCoordinator.Common;
using EventCoordinator.Event.Model;
using EventCoordinator.ProcessManager.Impl;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DemoWebApi.BusinessService.Impl
{
    public class UserEventHandler : EventProcessHandlerBase, IUserEventHandler
    {
        private readonly ILogger<UserEventHandler> logger;
        public UserEventHandler(ILogger<UserEventHandler> logger)
        {
            this.logger = logger;
        }
        [EventHandler(EventHandlerConfigure.EventTopic.ReduceUserBalance)]
        public async Task ReduceUserBalance(ReduceUserBalanceEvent input)
        {
            //随机模拟50%概率回退
            if (RandomHelper.GetRandomBool())
            {
                var data = new CreateOrderEvent();
                data.OrderNo = input.OrderNo;
                logger.LogInformation($"用户余额扣款成功,订单编号:{input.OrderNo},用户需支付余额:{input.Balance}!");
                await CallNext(data);
            }
            else
            {
                logger.LogWarning("用户余额扣款失败!");
                await RollBack();
            }
        }

        [EventHandler(EventHandlerConfigure.EventTopic.RollBackUserBalance)]
        public async Task RollBackUserBalance(ReduceUserBalanceEvent input)
        {
            //随机模拟50%概率补偿失败
            if (RandomHelper.GetRandomBool())
            {
                logger.LogInformation($"用户余额回滚成功!,订单编号:{input.OrderNo},用户需支付余额:{input.Balance}!");
            }
            else
            {
                logger.LogError($"用户余额回滚异常!");
                await RollBack();
            }
        }
    }
}
