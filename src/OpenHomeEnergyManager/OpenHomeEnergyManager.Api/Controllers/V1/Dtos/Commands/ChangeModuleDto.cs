using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Api.Controllers.V1.Dtos.Commands
{
    public class ChangeModuleDto
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
    }
}
