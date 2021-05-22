using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using OpenHomeEnergyManager.Domain.Model.ModuleAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.EntityFramework.Configurations
{
    class ModuleConfiguration : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> configuration)
        {
            configuration.ToTable("Modules");

            //configuration.HasKey(b => b.Id);

            //configuration.Property(b => b.Id);
            //configuration.Property(b => b.Name);
            //configuration.Property(b => b.ModuleServiceDefinitionKey);
            configuration.Property(b => b.Settings)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
        }
    }
}
