﻿using Consul;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceDiscoveryWebApi.Services
{
    public class AgentServiceRegistrationHostedService : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly AgentServiceRegistration _serviceRegistration;

        public AgentServiceRegistrationHostedService(
            IConsulClient consulClient,
            AgentServiceRegistration serviceRegistration)
        {
            _consulClient = consulClient;
            _serviceRegistration = serviceRegistration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _consulClient.Agent.ServiceRegister(_serviceRegistration, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _consulClient.Agent.ServiceDeregister(_serviceRegistration.ID, cancellationToken);
        }
    }
}
