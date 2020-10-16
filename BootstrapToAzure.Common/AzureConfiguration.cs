using System;
using System.Collections.Generic;
using System.Text;

namespace BootstrapToAzure.Common
{
    public class AzureConfiguration
    {
        public static string SectionName = "AzureConfiguration";

        public string AzureBlobConnectionString { get; set; }
    }
}
