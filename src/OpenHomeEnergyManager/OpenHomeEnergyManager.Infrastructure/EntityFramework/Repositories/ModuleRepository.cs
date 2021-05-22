using Microsoft.EntityFrameworkCore;
using OpenHomeEnergyManager.Domain.Model.ModuleAggregate;
using OpenHomeEnergyManager.Domain.SeedWork;
using OpenHomeEnergyManager.Infrastructure.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.EntityFramework.Repositories
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly OpenHomeEnergyManagerDbContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public ModuleRepository(OpenHomeEnergyManagerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public Module Add(Module module)
        {
            return _context.Modules.Add(module).Entity;
        }

        public IEnumerable<Module> GetAll()
        {
            var modules = _context
                    .Modules
                    .AsEnumerable();

            return modules.ToArray();
        }

        public void Remove(Module module)
        {
            _context.Modules.Remove(module);
        }

        public Module Update(Module module)
        {
            _context.Entry(module).State = EntityState.Modified;

            return module;
        }

        public async Task<Module> FindByIdAsync(int id)
        {
            var module = await _context
                                .Modules
                                .SingleOrDefaultAsync(c => c.Id == id);

            return module;
        }
    }
}
