using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Api.Controllers.V1.Dtos
{
    public class VehicleDto
    {
        public string Name { get; private set; }

        public string Image { get; private set; }

        public int ModuleId { get; set; }

        public bool ChargerMustBeOffOnChanges { get; set; }
    }
}
