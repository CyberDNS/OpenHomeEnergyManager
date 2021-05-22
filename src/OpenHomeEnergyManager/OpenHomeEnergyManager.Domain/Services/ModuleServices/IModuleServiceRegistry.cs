using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ModuleServices
{
    public interface IModuleServiceRegistry
    {
        public IModuleService FindById(int id);
    }
}
