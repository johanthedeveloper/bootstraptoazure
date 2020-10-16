using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BootstrapToAzure.Business;

namespace BootstrapToAzure.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IMainHandler mainHandler;

        public Worker(ILogger<Worker> logger, IMainHandler mainHandler)
        {
            this.logger = logger;
            this.mainHandler = mainHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await mainHandler.Run(stoppingToken);
            }
        }
    }
}
