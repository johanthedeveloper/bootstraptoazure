using BootstrapToAzure.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace BootstrapToAzure.Data
{
    public class DockerHandler : IDockerHandler
    {
        private ILogger<DockerHandler> logger;
        DockerResultModel dockerResultModel;

        public DockerHandler(ILogger<DockerHandler> logger)
        {
            this.logger = logger;
        }

        public DockerResultModel StartContainer(string containerName)
        {
            return RunCommand($"start {containerName}");
        }

        public DockerResultModel StopContainer(string containerName)
        {
            return RunCommand($"stop {containerName}");
        }

        private DockerResultModel RunCommand (string command)
        {
            dockerResultModel = new DockerResultModel();

            var processInfo = new ProcessStartInfo("docker", command);

            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            using (var process = new Process())
            {
                process.StartInfo = processInfo;
                process.OutputDataReceived += Process_OutputDataReceived;
                process.ErrorDataReceived += Process_ErrorDataReceived;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit(120000);
                if (!process.HasExited)
                {
                    process.Kill();
                }

                dockerResultModel.ExitCode = process.ExitCode;
                process.Close();
                dockerResultModel.ExitWithoutError = true;

                if (dockerResultModel.ExitCode != 0)
                {
                    dockerResultModel.ExitWithoutError = false;
                    logger.LogWarning($"Exit code was '{dockerResultModel.ExitCode}' of programm 'docker {command}'");
                }

                for (int i = 0; i < 10; i++)
                {
                    if(!string.IsNullOrEmpty(dockerResultModel.MessageResult) && !string.IsNullOrEmpty(dockerResultModel.MessageError))
                    {
                        break;
                    }

                    logger.LogDebug("Sleeping (waiting for process to exit)");
                   Task.Delay(1000).GetAwaiter().GetResult();
                }

                return dockerResultModel;
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            LogInformation("Process output (ERROR)", e.Data);

            dockerResultModel.MessageError = ConcatMessages(dockerResultModel.MessageError, e.Data);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            LogInformation("Process output", e.Data);

            dockerResultModel.MessageError = ConcatMessages(dockerResultModel.MessageResult, e.Data);
        }

        private string ConcatMessages(string message, string newMessagePart)
        {
            if(string.IsNullOrEmpty(message))
            {
                return newMessagePart;
            }

            return $"{message}\r\n{newMessagePart}";
        }

        private void LogInformation(string mainMessage, string data)
        {
            if(!string.IsNullOrEmpty(data))
            {
                logger.LogInformation($"{mainMessage}: {data}");
            }
        }
    }
}
