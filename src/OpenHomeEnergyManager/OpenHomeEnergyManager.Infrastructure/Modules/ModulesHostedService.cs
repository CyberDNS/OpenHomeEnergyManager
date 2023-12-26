using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenHomeEnergyManager.Domain.Model.ModuleAggregate;
using OpenHomeEnergyManager.Domain.Model.ModuleServiceDefinitionAggregate;
using OpenHomeEnergyManager.Domain.Services.ModuleServices;
using OpenHomeEnergyManager.Infrastructure.EntityFramework;
using OpenHomeEnergyManager.Infrastructure.Modules.OpenWb;
using OpenHomeEnergyManager.Infrastructure.Modules.SmaHomeManager;
using OpenHomeEnergyManager.Infrastructure.Modules.SmaTriPower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules
{
    class ModulesHostedService : IHostedService
    {
        private readonly ILogger<ModulesHostedService> _logger;
        private readonly IServiceProvider _services;
        private readonly ModuleServiceRegistry _serviceRegistry;

        public ModulesHostedService(ILogger<ModulesHostedService> logger, IServiceProvider services, IModuleServiceRegistry serviceRegistry)
        {
            _logger = logger;
            _services = services;
            _serviceRegistry = (ModuleServiceRegistry)serviceRegistry;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _services.CreateScope())
            {
                var modulesRepository = scope.ServiceProvider.GetRequiredService<IModuleRepository>();
                var moduleServiceDefinitionsRepository = scope.ServiceProvider.GetRequiredService<IModuleServiceDefinitionRepository>();
                var moduleServiceDefinitions = moduleServiceDefinitionsRepository.GetAll().ToDictionary(d => d.Key);

                foreach (var module in modulesRepository.GetAll().Where(m => m.ModuleServiceDefinitionKey is not null))
                {
                    if (moduleServiceDefinitions.ContainsKey(module.ModuleServiceDefinitionKey))
                    {
                        var moduleServiceDefinition = moduleServiceDefinitions[module.ModuleServiceDefinitionKey];
                        _serviceRegistry.TryRegister(moduleServiceDefinition.Type, module);
                    }
                    else
                    {
                        _logger.LogWarning("Module with {ModuleServiceDefinitionKey} not found in ModuleServiceDefinitions", module.ModuleServiceDefinitionKey);
                    }
                }
            }

            foreach (IHostedModuleService service in _serviceRegistry.ModuleServices.Values.OfType<IHostedModuleService>())
            {
                await service.StartAsync(cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (IHostedModuleService service in _serviceRegistry.ModuleServices.Values.OfType<IHostedModuleService>())
            {
                await service.StopAsync(cancellationToken);
            }
        }
    }
}
