using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules.Tesla.Authentication.Dtos
{
    public class ChargeStateDto
    {
        [JsonPropertyName("response")]
        public ResponseDto Response { get; set; }

        public class ResponseDto
        {
            [JsonPropertyName("battery_level")]
            public int BatteryLevel { get; set; }

            [JsonPropertyName("charging_state")]
            public string ChargingState { get; set; }

            [JsonPropertyName("charge_limit_soc")]
            public int ChargeLimitSoc { get; set; }
        }
    }
}
