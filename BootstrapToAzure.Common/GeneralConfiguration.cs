using System;
using System.Collections.Generic;
using System.Text;

namespace BootstrapToAzure.Common
{
    public class GeneralConfiguration
    {
        public static string SectionName = "GeneralConfiguration";

        public int StartupWaitingTimeInMinutes { get; set; }
    }
}
