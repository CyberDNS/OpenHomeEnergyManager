using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules.Queries;

namespace OpenHomeEnergyManager.Blazor.Components.VehicleManagement.Settings
{
    public class VehicleSettingsModel
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public int? ModuleId { get; set; }

        private ModuleDto _module;
        public ModuleDto Module
        {
            get => _module;
            set
            {
                _module = value;
                if (_module is not null) { ModuleId = _module.Id; }
            }
        }

        public bool ChargerMustBeOffOnChanges { get; set; }
    }
}
