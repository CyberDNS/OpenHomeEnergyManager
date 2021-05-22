using OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ModuleServices
    
{
    public interface IModuleService
    {
        public IEnumerable<Capability> Capabilities { get; }

        public void Configure(IDictionary<string, string> settings);
    }
}