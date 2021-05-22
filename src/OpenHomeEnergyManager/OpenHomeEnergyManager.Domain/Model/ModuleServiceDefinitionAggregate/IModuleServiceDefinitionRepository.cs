using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Model.ModuleServiceDefinitionAggregate
{
    public interface IModuleServiceDefinitionRepository
    {
        public IEnumerable<IModuleServiceDefinition> GetAll();
    }
}
