using DemoWebApi.BusinessService.Dtos;
using DemoWebApi.Common;
using EventCoordinator.ProcessManager.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class MockCreateOrderEventProcessController : ControllerBase
    {
        private readonly IProcessManager processManager;
        public MockCreateOrderEventProcessController(IProcessManager processManager)
        {
            this.processManager = processManager;
        }
        [HttpGet]
        public async Task<string> Get()
        {
            await processManager.GetAndRun(EventHandlerConfigure.ProcessManagerPipline.OrderCreate,
                new PrepareCreateOrderEvent() { produceItems = new List<PrepareCreateOrderEvent.BuyProduceItem>() { new PrepareCreateOrderEvent.BuyProduceItem() { ProductId = 1, Count = 3 } } });
            return "ok";
        }
    }
}
