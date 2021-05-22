using OpenHomeEnergyManager.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Model.ModuleAggregate
{
    public interface IModuleRepository : IRepository<Module>
    {
        Module Add(Module module);
        Module Update(Module module);

        void Remove(Module module);

        IEnumerable<Module> GetAll();

        Task<Module> FindByIdAsync(int id);
    }
}
