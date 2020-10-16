using System.Threading;
using System.Threading.Tasks;

namespace BootstrapToAzure.Business
{
    public interface IMainHandler
    {
        Task Run(CancellationToken stoppingToken);
    }
}