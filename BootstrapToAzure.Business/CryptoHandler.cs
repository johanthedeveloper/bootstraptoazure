using Azure.Storage.Blobs.Models;
using BootstrapToAzure.Common;
using BootstrapToAzure.Data;
using BootstrapToAzure.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BootstrapToAzure.Business
{
    public class CryptoHandler : ICryptoHandler
    {
        private ILogger<CryptoHandler> logger;
        private IBlobUploadHandler blobUploadHandler;
        private IDockerHandler dockerHandler;
        private IFileHandler fileHandler;

        public CryptoHandler(ILogger<CryptoHandler> logger, IBlobUploadHandler blobUploadHandler, IDockerHandler dockerHandler, IFileHandler fileHandler)
        {
            this.logger = logger;
            this.blobUploadHandler = blobUploadHandler;
            this.dockerHandler = dockerHandler;
            this.fileHandler = fileHandler;
        }

        public void Run(BaseCryptoConfiguration cryptoConfiguration)
        {
            if (cryptoConfiguration.Enabled)
            {
                bool restarted = false;
                try
                {
                    logger.LogInformation($"Stopping docker container {cryptoConfiguration.DockerContainerName}");
                    DockerResultModel dockerResultModel = dockerHandler.StopContainer(cryptoConfiguration.DockerContainerName);
                    logger.LogInformation($"Status: {dockerResultModel.ExitCode}");

                    //Remove before starting
                    logger.LogInformation($"Deleting old boostrap directory and boostrap zip file in directory {cryptoConfiguration.CryptoLocalDirectoryFullName}");
                    fileHandler.DeleteBoostrapDirectory(cryptoConfiguration.CryptoLocalDirectoryFullName);
                    fileHandler.DeleteZipBoostrap(cryptoConfiguration.CryptoLocalDirectoryFullName);

                    logger.LogInformation($"Creating boostrap directory on {cryptoConfiguration.CryptoLocalDirectoryFullName}");
                    fileHandler.CreateBootstrapDirectory(cryptoConfiguration.CryptoLocalDirectoryFullName);

                    logger.LogInformation($"Copy files to boostrap directory on {cryptoConfiguration.CryptoLocalDirectoryFullName}");
                    if (fileHandler.ExcistsDirectory(cryptoConfiguration.CryptoLocalDirectoryFullName, "blocks/") && fileHandler.ExcistsDirectory(cryptoConfiguration.CryptoLocalDirectoryFullName, "chainstate/"))
                    {
                        logger.LogInformation("Going for block with chainstate");
                        fileHandler.CopyFilesToBootstrap(cryptoConfiguration.CryptoLocalDirectoryFullName, "blocks/");
                        fileHandler.CopyFilesToBootstrap(cryptoConfiguration.CryptoLocalDirectoryFullName, "chainstate/");
                    }
                    else
                    {
                        logger.LogInformation("Going for blk*.dat with txtlevldb");
                        fileHandler.CopyFilesToBootstrap(cryptoConfiguration.CryptoLocalDirectoryFullName, "blk*.dat");
                        fileHandler.CopyFilesToBootstrap(cryptoConfiguration.CryptoLocalDirectoryFullName, "txleveldb/");
                    }

                    RestartDockerContainer(cryptoConfiguration.DockerContainerName);
                    restarted = true;

                    logger.LogInformation($"Start zipping boostrap");
                    string fullFileName = fileHandler.ZipBootstrap(cryptoConfiguration.CryptoLocalDirectoryFullName);

                    logger.LogInformation($"Start uploading");
                    blobUploadHandler.UploadBlob(cryptoConfiguration.AzureBlobContainerName, $"bootstrap-{DateTime.Now.ToString("yyyyMMdd")}.zip", fullFileName, AccessTier.Cool);
                    blobUploadHandler.UploadBlob(cryptoConfiguration.AzureBlobContainerName, "bootstrap.zip", fullFileName, AccessTier.Hot);

                    //Remove after finishing
                    logger.LogInformation($"Deleting directory and files {cryptoConfiguration.CryptoLocalDirectoryFullName}");
                    fileHandler.DeleteBoostrapDirectory(cryptoConfiguration.CryptoLocalDirectoryFullName);
                    fileHandler.DeleteZipBoostrap(cryptoConfiguration.CryptoLocalDirectoryFullName);
                }
                catch(Exception ex)
                {
                    logger.LogError($"Error: {ex.Message}");
                    throw;
                }
                finally
                {
                    if (!restarted)
                    {
                        RestartDockerContainer(cryptoConfiguration.DockerContainerName);
                    }
                }
            }
            else
            {
                logger.LogInformation($"{cryptoConfiguration.DockerContainerName} is disabled!");
            }
        }

        private void RestartDockerContainer(string dockerContainerName)
        {
            logger.LogInformation($"Starting docker container {dockerContainerName}");
            DockerResultModel dockerResultModel = dockerHandler.StartContainer(dockerContainerName);
            logger.LogInformation($"Status: {dockerResultModel.ExitCode}");
        }
    }
}
