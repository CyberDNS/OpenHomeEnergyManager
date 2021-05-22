using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet.Client.Options;
using OpenHomeEnergyManager.Blazor.Infrastructure.Mqtt.Options;
using OpenHomeEnergyManager.Blazor.Infrastructure.Mqtt.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.Mqtt.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddMqttClientService(this IServiceCollection services, Action<AspCoreMqttClientOptionBuilder> configure)
        {
            services.AddSingleton(serviceProvider =>
            {
                var optionBuilder = new AspCoreMqttClientOptionBuilder(serviceProvider);
                configure(optionBuilder);
                return optionBuilder.Build();
            });
            services.AddSingleton<MqttManagementService>();
            services.AddSingleton<IHostedService>(serviceProvider =>
            {
                return serviceProvider.GetService<MqttManagementService>();
            });
            services.AddSingleton(serviceProvider =>
            {
                var mqttClientService = serviceProvider.GetService<MqttManagementService>();
                var mqttClientServiceProvider = new MqttManagementServiceProvider(mqttClientService);
                return mqttClientServiceProvider;
            });
            return services;
        }
    }
}
