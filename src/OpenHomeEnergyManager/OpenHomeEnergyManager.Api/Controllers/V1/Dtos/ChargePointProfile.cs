using AutoMapper;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Api.Controllers.V1.Dtos
{
    public class ChargePointProfile : Profile
    {
        public ChargePointProfile()
        {
            CreateMap<ChargePoint, ChargePointDto>();
            CreateMap<ChargePointDto, ChargePoint>();
        }
    }
}
