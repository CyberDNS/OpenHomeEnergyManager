using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules.Tesla.Authentication
{
    public class TokenMaterial
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
