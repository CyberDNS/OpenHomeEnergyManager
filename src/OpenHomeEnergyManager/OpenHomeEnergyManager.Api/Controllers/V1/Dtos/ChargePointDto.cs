using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Api.Controllers.V1.Dtos
{
    public class ChargePointDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public int ModuleId { get; set; }

        public int Power { get; }

        public int Current { get; }
    }
}
