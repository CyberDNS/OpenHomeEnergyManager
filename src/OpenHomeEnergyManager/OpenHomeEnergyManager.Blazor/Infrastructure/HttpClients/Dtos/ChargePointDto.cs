using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Dtos
{
    public class ChargePointDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Name length can't be more than 20.")]
        public string Name { get; set; }

        [Required]
        public int ModuleId { get; set; }

        public string Image { get; set; }

        public int VehicleId { get; set; }

        public string CurrentChargeMode { get; set; }
    }
}
