using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.Mqtt.Services
{
    public class MqttManagementServiceProvider
    {
        public readonly IMqttManagementService MqttManagementService;

        public MqttManagementServiceProvider(IMqttManagementService mqttManagementService)
        {
            MqttManagementService = mqttManagementService;
        }
    }
}
