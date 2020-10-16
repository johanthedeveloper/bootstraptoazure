using System;
using System.Collections.Generic;
using System.Text;

namespace BootstrapToAzure.Common
{
    public class BaseCryptoConfiguration
    {
        public bool Enabled { get; set; }
        public string AzureBlobContainerName { get; set; }
        public string CryptoLocalDirectoryFullName { get; set; }
        public string DockerContainerName { get; set; }
    }
}
