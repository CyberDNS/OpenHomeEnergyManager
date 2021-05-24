using OpenHomeEnergyManager.Domain.Services.ModuleServices;
using OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenHomeEnergyManager.Infrastructure.Modules
{
    public abstract class ModuleServiceBase : IModuleService
    {
        protected Dictionary<string, Capability> _capabilities = new Dictionary<string, Capability>();
        public IEnumerable<Capability> Capabilities => _capabilities.Values;

        public abstract void Configure(IDictionary<string, string> settings);

        public void RegisterCapability<T>(T capability) where T : Capability
        {
            if (_capabilities.ContainsKey(capability.Key)) { throw new ArgumentException($"Capability key {capability.Key} already registred"); }
            _capabilities.Add(capability.Key, capability);
        }

        public T GetCapability<T>(string key) where T : Capability
        {
            if (!_capabilities.ContainsKey(key)) { return null; }

            var capability = _capabilities[key];
            if (capability is not T) { return null; }

            return (T)capability;
        }
    }
}