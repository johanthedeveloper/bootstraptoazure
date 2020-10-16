using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BootstrapToAzure.Data
{
    public class FileHandler : IFileHandler
    {
        private const string bootstrapDirectoryName = "bootstrap";
        private const string bootstrapFileName = "bootstrap.zip";

        private ILogger<FileHandler> logger;

        public FileHandler(ILogger<FileHandler> logger)
        {
            this.logger = logger;
        }

        public void CopyFilesToBootstrap(string rootPath, string patternFilesToCopy)
        {
            DirectoryInfo rootPathDirectory = new DirectoryInfo(rootPath);
            DirectoryInfo bootstrapDirectory = GetDirectoryInfoBoostrap(rootPath);

            logger.LogDebug($"Starting copy files from '{rootPathDirectory.FullName}' to bootstrap '{bootstrapDirectory.FullName}'");

            foreach (FileInfo fileInfo in rootPathDirectory.GetFiles(patternFilesToCopy))
            {
                string newPartyFileName = fileInfo.FullName.Replace(rootPath, "");
                string newFullFileName = $"{bootstrapDirectory.FullName}{newPartyFileName}";

                FileInfo newFile = new FileInfo(newFullFileName);
                DirectoryInfo newDirectory = newFile.Directory;
                if (!newDirectory.Exists)
                {
                    logger.LogDebug($"Creating directory '{newDirectory.FullName}'");
                    newDirectory.Create();
                }

                logger.LogDebug($"Starting copy file '{fileInfo.FullName}' to '{newFullFileName}'");

                fileInfo.CopyTo(newFullFileName);
            }
        }

        public void CreateBootstrapDirectory(string rootPath)
        {
            DirectoryInfo directoryBootstrap = GetDirectoryInfoBoostrap(rootPath);
            if (!directoryBootstrap.Exists)
            {
                logger.LogDebug($"Creating bootstrap directory '{directoryBootstrap.FullName}'");
                directoryBootstrap.Create();
            }
        }

        public void DeleteBoostrapDirectory(string rootPath)
        {
            DirectoryInfo directoryBootstrap = GetDirectoryInfoBoostrap(rootPath);
            if (directoryBootstrap.Exists)
            {
                logger.LogDebug($"Deleting bootstrap directory '{directoryBootstrap.FullName}'");
                directoryBootstrap.Delete(true);
            }
        }
        public void DeleteZipBoostrap(string rootPath)
        {
            FileInfo file = GetFileInfoBoostrap(rootPath);
            if (file.Exists)
            {
                logger.LogDebug($"Creating bootstrap file '{file.FullName}'");
                file.Delete();
            }
        }

        public bool ExcistsDirectory(string rootPath, string directoryName)
        {
            string extraSlash = "/";

            if (rootPath.EndsWith("/"))
            {
                extraSlash = "";
            }

            if (!directoryName.EndsWith("/"))
            {
                directoryName += "/";
            }

            DirectoryInfo directoryInfo = new DirectoryInfo($"{rootPath}{extraSlash}{directoryName}");

            return directoryInfo.Exists;
        }

        private DirectoryInfo GetDirectoryInfoBoostrap(string rootPath)
        {
            string extraSlash = "/";

            if (rootPath.EndsWith("/"))
            {
                extraSlash = "";
            }

            return  new DirectoryInfo($"{rootPath}{extraSlash}{bootstrapDirectoryName}/");
        }

        private FileInfo GetFileInfoBoostrap(string rootPath)
        {
            string extraSlash = "/";

            if (rootPath.EndsWith("/"))
            {
                extraSlash = "";
            }

            return new FileInfo($"{rootPath}{extraSlash}{bootstrapFileName}");
        }

        public string ZipBootstrap(string rootPath)
        {
            DirectoryInfo directoryInfo = GetDirectoryInfoBoostrap(rootPath);
            FileInfo fileInfo = GetFileInfoBoostrap(rootPath);

            ZipFile.CreateFromDirectory(directoryInfo.FullName, fileInfo.FullName, CompressionLevel.Optimal, true);

            return fileInfo.FullName;
        }
    }
}
