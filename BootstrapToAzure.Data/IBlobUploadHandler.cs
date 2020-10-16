using Azure.Storage.Blobs.Models;
using System;

namespace BootstrapToAzure.Data
{
    public interface IBlobUploadHandler
    {
        DateTime GetLastModifiedForFile(string containerName, string fileNameInAzure);

        void UploadBlob(string containerName, string fileNameInAzure, string localFullFileName, AccessTier accessTier);
    }
}