using BootstrapToAzure.Data.Models;

namespace BootstrapToAzure.Data
{
    public interface IDockerHandler
    {
        DockerResultModel StartContainer(string containerName);

        DockerResultModel StopContainer(string containerName);
    }
}