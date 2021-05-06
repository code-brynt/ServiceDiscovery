using Consul;

namespace ServiceDiscoveryWebApi.Interfaces
{
    public interface IConsulClientFactory
    {
        IConsulClient CreateClient();
        IConsulClient CreateClient(string name);
    }
}
