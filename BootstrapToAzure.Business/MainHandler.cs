using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BootstrapToAzure.Data;
using Microsoft.Extensions.Options;
using BootstrapToAzure.Common;

namespace BootstrapToAzure.Business
{
    public class MainHandler : IMainHandler
    {
        private ILogger<MainHandler> logger;
        private ICryptoHandler cryptoHandler;
        private GeneralConfiguration generalConfiguration;
        private VericoinConfiguration vericoinConfiguration;
        private VeriumConfiguration veriumConfiguration;

        public MainHandler(ILogger<MainHandler> logger, ICryptoHandler cryptoHandler, IOptions<GeneralConfiguration> optionsGeneralConfiguration, IOptions<VericoinConfiguration> optionsVericoinConfiguration, IOptions<VeriumConfiguration> optionsVeriumConfiguration)
        {
            this.logger = logger;
            this.cryptoHandler = cryptoHandler;
            this.generalConfiguration = optionsGeneralConfiguration.Value;
            this.vericoinConfiguration = optionsVericoinConfiguration.Value;
            this.veriumConfiguration = optionsVeriumConfiguration.Value;
        }

        private DateTime lastDateTimeLocalFile = new DateTime();

        public async Task Run(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting running");

            int startupWaitingTimeInMinutes = generalConfiguration.StartupWaitingTimeInMinutes * 60 * 1000;
            logger.LogInformation($"Startup waiting time is {generalConfiguration.StartupWaitingTimeInMinutes} minutes. Starting now.");
            await Task.Delay(startupWaitingTimeInMinutes, stoppingToken);
            logger.LogInformation("Finish startup waiting time");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    cryptoHandler.Run(vericoinConfiguration);
                    cryptoHandler.Run(veriumConfiguration);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error: {ex.Message}");
                }

                await SleepUntilNextTimeslot(stoppingToken);
            }

            logger.LogInformation("Finished running");
        }

        private async Task SleepUntilNextTimeslot(CancellationToken stoppingToken)
        {
            DateTime dateTimeNextTimeslot = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(1);
            TimeSpan timespanToNextTimeslot = dateTimeNextTimeslot.Subtract(DateTime.Now);
            int sleepTimeInSeconds = (int) timespanToNextTimeslot.TotalSeconds;


            logger.LogInformation($"Start sleeping for {sleepTimeInSeconds} seconds");
            await Task.Delay(sleepTimeInSeconds * 1000, stoppingToken);
        }
    }
}


/*
DateTime currentDateTimeLocalFile = fileHandler.GetLastWriteTimeUtc(localFullFileName);

if (lastDateTimeLocalFile < currentDateTimeLocalFile)
{
    DateTime currentDateTimeAzureFile = blobUploadHandler.GetLastModifiedForFile(fileNameInAzure);

    if (currentDateTimeLocalFile > currentDateTimeAzureFile)
    {
        logger.LogInformation("File local is newer then server. Start uploading");
        blobUploadHandler.UploadBlob(string.Format(fileNameInAzureDay, DateTime.Now.ToString("yyyyMMdd")), localFullFileName, AccessTier.Cool);
        blobUploadHandler.UploadBlob(fileNameInAzure, localFullFileName, AccessTier.Hot);
    }
    else
    {
        logger.LogInformation("File is already up to date (server)");
    }

    lastDateTimeLocalFile = currentDateTimeLocalFile;
}
else
{
    logger.LogInformation("File is already up to date (local)");
} 
*/
