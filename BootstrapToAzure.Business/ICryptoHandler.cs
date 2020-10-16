using BootstrapToAzure.Common;

namespace BootstrapToAzure.Business
{
    public interface ICryptoHandler
    {
        void Run(BaseCryptoConfiguration cryptoConfiguration);
    }
}