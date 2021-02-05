using Autofac;
using Autofac.Extensions.DependencyInjection;
using DemoWebApi.BusinessService.Impl;
using DemoWebApi.BusinessService.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterModule(new EventCoordinator.Common.Module());

                    builder.RegisterType<OrderEventHandler>().As<IOrderEventHandler>().InstancePerLifetimeScope();
                    builder.RegisterType<ProductEventHandler>().As<IProductEventHandler>().InstancePerLifetimeScope();
                    builder.RegisterType<UserEventHandler>().As<IUserEventHandler>().InstancePerLifetimeScope();
                })
                .ConfigureServices(services =>
                {
                    services.AddAutofac();
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
