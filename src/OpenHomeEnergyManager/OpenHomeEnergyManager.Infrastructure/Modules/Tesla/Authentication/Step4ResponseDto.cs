﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenHomeEnergyManager.Infrastructure.Modules.Tesla.Authentication
{
    class Step4ResponseDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}
