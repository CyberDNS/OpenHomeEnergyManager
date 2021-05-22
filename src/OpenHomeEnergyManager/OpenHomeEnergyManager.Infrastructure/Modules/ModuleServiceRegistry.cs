using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using OpenHomeEnergyManager.Domain.Model.ModuleAggregate;
using OpenHomeEnergyManager.Domain.Services.ModuleServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules
{
    public class ModuleServiceRegistry : IModuleServiceRegistry
    {
        private Dictionary<int, IModuleService> _moduleServices = new Dictionary<int, IModuleService>();
        private readonly IServiceProvider _serviceProvider;

        public IReadOnlyDictionary<int, IModuleService> ModuleServices => _moduleServices;

        public ModuleServiceRegistry(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void TryRegister<T>(Module module) where T : IModuleService
        {
            TryRegister(typeof(T), module);
        }

        public void TryRegister(Type type, Module module)
        {
            if (!_moduleServices.ContainsKey(module.Id))
            {
                IModuleService moduleService = (IModuleService)_serviceProvider.GetRequiredService(type);
                moduleService.Configure(module.Settings);
                _moduleServices.Add(module.Id, moduleService);
            }
        }

        public IModuleService FindById(int id)
        {
            if (!_moduleServices.ContainsKey(id)) { return null; }

            return _moduleServices[id];
        }
    }
}
