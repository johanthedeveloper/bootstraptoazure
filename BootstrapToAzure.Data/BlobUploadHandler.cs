using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BootstrapToAzure.Common;
using Microsoft.Extensions.Logging;

namespace BootstrapToAzure.Data
{
    public class BlobUploadHandler : IBlobUploadHandler
    {
        private ILogger<BlobUploadHandler> logger;
        private AzureConfiguration azureConfiguration;
        private BlobServiceClient blobServiceClient;

        public BlobUploadHandler(ILogger<BlobUploadHandler> logger, IOptions<AzureConfiguration> options, IConfiguration configuration)
        {
            this.logger = logger;
            this.azureConfiguration = options.Value;

            blobServiceClient = new BlobServiceClient(azureConfiguration.AzureBlobConnectionString);
        }

        public DateTime GetLastModifiedForFile(string containerName, string fileNameInAzure)
        {
            if (string.IsNullOrEmpty(fileNameInAzure))
            {
                throw new ArgumentException("FileNameInAzure cannot be empty");
            }

            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileNameInAzure);
            BlobProperties blobProperties = blobClient.GetProperties();

            return blobProperties.LastModified.UtcDateTime;
        }

        public void UploadBlob(string containerName, string fileNameInAzure, string localFullFileName, AccessTier accessTier)
        {
            if (string.IsNullOrEmpty(fileNameInAzure))
            {
                throw new ArgumentException("FileNameInAzure cannot be empty");
            }

            if (string.IsNullOrEmpty(localFullFileName))
            {
                throw new ArgumentException("LocalFullFileName cannot be empty");
            }

            if (!File.Exists(localFullFileName))
            {
                throw new IOException($"File '{localFullFileName}' is not there");
            }

            // Create a blob container client
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

#if DEBUG
            fileNameInAzure = $"dev-{fileNameInAzure}";
#endif

            
            // Create a blob container and a new block blob
            BlockBlobClient blobClient = blobContainerClient.GetBlockBlobClient(fileNameInAzure);

            byte[] data = new byte[1024 * 1024];
            int blockNumber = 0;
            List<string> blockIds = new List<string>();
            int currentPercentageUpload = -1;

            using FileStream uploadFileStream = File.OpenRead(localFullFileName);
            {
                while (uploadFileStream.Position < uploadFileStream.Length)
                {
                    try
                    {
                        int dataToSend = uploadFileStream.Read(data, 0, data.Length);

                        byte[] firstBlockID = Encoding.UTF8.GetBytes(blockNumber.ToString().PadLeft(10, '0'));
                        string firstIDBase64 = Convert.ToBase64String(firstBlockID); // "MA=="
                        var stageResponse = blobClient.StageBlock(firstIDBase64, new MemoryStream(data, 0, dataToSend));
                        var responseInfo = stageResponse.GetRawResponse(); // 201: Created

                        blockIds.Add(firstIDBase64);

                        int percentageUpload = (int)(uploadFileStream.Position * 100 / uploadFileStream.Length);
                        if (currentPercentageUpload < percentageUpload)
                        {
                            currentPercentageUpload = percentageUpload;
                            logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Uploading file '{fileNameInAzure}' ({percentageUpload}%)");
                        }

                        blockNumber++;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Exception occured during uploading of local file '{localFullFileName}' to azure file '{fileNameInAzure}'", ex);
                    }
                }
            }

            blobClient.CommitBlockList(blockIds);

            blobClient.SetAccessTier(accessTier);
        }
    }
}
