using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FileWatcherService.Clases;

namespace FileWatcherService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Watcher w1 = new Watcher(@"C:\Users\ssoch\Desktop\TDS\Parameters", "*.txt");
           // while (!stoppingToken.IsCancellationRequested)
           // {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    w1.StartWatch();
                    _logger.LogInformation($"Ejecuto Watcher");
                } catch (Exception ex)
                {
                    _logger.LogError($"Error: {ex.Message}");
                }
            _logger.LogInformation($"Worker completed at: {DateTimeOffset.Now}");
           // await Task.Delay(1000, stoppingToken);
           // }
        }
    }
}
