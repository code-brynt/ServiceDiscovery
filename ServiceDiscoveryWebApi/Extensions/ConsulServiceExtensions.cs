﻿using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;

namespace ServiceDiscoveryWebApi.Extensions
{
    public static class ConsulServiceExtensions
    {
        public static IServiceCollection AddConsulConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var host = configuration.GetValue<string>("ConsulConfig:ConsulHost");
                consulConfig.Address = new Uri(host);
            }));

            return services;
        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IConfiguration configuration)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("AppExtensions");
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            if (app.Properties["server.Features"] is not FeatureCollection features)
            {
                return app;
            }

            var servicePort = int.Parse(configuration.GetValue<string>("ConsulConfig:ServicePort"));
            var serviceIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            var serviceName = configuration.GetValue<string>("ConsulConfig:ServiceName");
            var serviceId = configuration.GetValue<string>("ConsulConfig:ServiceId");

            var registration = new AgentServiceRegistration()
            {
                ID = serviceId,
                Name = serviceName,
                Address = serviceIp.ToString(),
                Port = servicePort,

                Check = new AgentCheckRegistration()
                {
                    HTTP = $"https://localhost:{servicePort}/api/health/status",
                    Interval = TimeSpan.FromSeconds(10)
                }
            };

            logger.LogInformation("Registering with Consul");
            consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            consulClient.Agent.ServiceRegister(registration).ConfigureAwait(true);

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Unregistering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            });

            return app;
        }
    }
}
