using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.EntityFramework.Configurations
{
    class ChargePointConfiguration : IEntityTypeConfiguration<ChargePoint>
    {
        public void Configure(EntityTypeBuilder<ChargePoint> chargePointConfiguration)
        {
            chargePointConfiguration.ToTable("ChargePoints");
        }
    }
}
