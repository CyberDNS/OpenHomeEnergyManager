using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace OpenHomeEnergyManager.Api.Controllers.V1.ChargePoints.Commands
{
    public class SetCurrentDto
    {
        [FromBody, Range(0, 32)]
        public int Current { get; set; }
    }
}
