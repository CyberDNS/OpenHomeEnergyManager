using Microsoft.Extensions.DependencyInjection;
using OpenHomeEnergyManager.Domain.Model.ModuleServiceDefinitionAggregate;
using OpenHomeEnergyManager.Domain.Services.ModuleServices;
using OpenHomeEnergyManager.Infrastructure.Modules.OpenWb;
using OpenHomeEnergyManager.Infrastructure.Modules.SmaHomeManager;
using OpenHomeEnergyManager.Infrastructure.Modules.SmaTriPower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules.DependencyInjection
{
    public static class ModulesServiceCollectionExtensions
    {
        public static IServiceCollection AddModuleServices(this IServiceCollection services)
        {
            services.AddSingleton<IModuleServiceRegistry, ModuleServiceRegistry>();
            services.AddHostedService<ModulesHostedService>();

            ModuleServiceDefinitionRepository repository = new();
            services.AddSingleton<IModuleServiceDefinitionRepository>(repository);

            foreach (var type in repository.GetAll().Select(d => d.Type))
            {
                services.AddTransient(type);
            }

            return services;
        }
    }
}
