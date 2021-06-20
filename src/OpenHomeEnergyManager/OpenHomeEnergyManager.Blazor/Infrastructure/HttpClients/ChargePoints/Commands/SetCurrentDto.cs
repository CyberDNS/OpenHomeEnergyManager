using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints.Commands
{
    public class SetCurrentDto
    {
        [FromBody, Range(0, 32)]
        public int Current { get; set; }
    }
}
