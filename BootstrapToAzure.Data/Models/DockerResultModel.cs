using System;
using System.Collections.Generic;
using System.Text;

namespace BootstrapToAzure.Data.Models
{
    public class DockerResultModel
    {
        public DockerResultModel()
        {

        }

        public int ExitCode { get; set; }
        
        public bool ExitWithoutError { get; set; }

        public string MessageError { get; set; }

        public string MessageResult { get; set; }
    }
}
