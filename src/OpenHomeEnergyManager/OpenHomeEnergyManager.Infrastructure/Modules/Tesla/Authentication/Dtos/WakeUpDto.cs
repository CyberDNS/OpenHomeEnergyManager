using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules.Tesla.Authentication.Dtos
{
    public class WakeUpDto
    {
        [JsonPropertyName("response")]
        public ResponseDto Response { get; set; }

        public class ResponseDto
        {
            [JsonPropertyName("state")]
            public string State { get; set; }
        }
    }
}
