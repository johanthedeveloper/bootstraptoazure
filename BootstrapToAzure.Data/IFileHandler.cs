using System;

namespace BootstrapToAzure.Data
{
    public interface IFileHandler
    {
        void CreateBootstrapDirectory(string rootPath);

        void DeleteBoostrapDirectory(string rootPath);

        bool ExcistsDirectory(string rootPath, string directoryName);

        string ZipBootstrap(string rootPath);

        void DeleteZipBoostrap(string rootPath);

        void CopyFilesToBootstrap(string rootPath, string patternFilesToCopy);

        //DateTime GetLastWriteTimeUtc(string localFullFileName);
    }
}