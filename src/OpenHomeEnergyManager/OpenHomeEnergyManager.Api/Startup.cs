using AutoMapper;
using InfluxDB.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Model.ModuleAggregate;
using OpenHomeEnergyManager.Domain.Model.VehicleAggregate;
using OpenHomeEnergyManager.Domain.Services.ChargeModesServices;
using OpenHomeEnergyManager.Domain.Services.ChargePointServices;
using OpenHomeEnergyManager.Domain.Services.VehicleServices;
using OpenHomeEnergyManager.Infrastructure.ChargeModes;
using OpenHomeEnergyManager.Infrastructure.EntityFramework;
using OpenHomeEnergyManager.Infrastructure.EntityFramework.Repositories;
using OpenHomeEnergyManager.Infrastructure.Modules.DependencyInjection;
using OpenHomeEnergyManager.Infrastructure.Modules.HomeAssistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(s => InfluxDBClientFactory.Create("http://localhost:8086", "09M9HVw9QbLCVgcGnqVbUkpB6yTXIJHx-r1T8TYiKXHhAM84ibm0gCMklPDbzfFc3WFNYn3v4jqdvAUAsFFmqg=="));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OpenHomeEnergyManager.Api", Version = "v1" });
            });

            services.AddDbContext<OpenHomeEnergyManagerDbContext>(options => options.UseSqlite("Data Source=../OpenHomeEnergyManager.db"));

            services.AddScoped<IChargePointRepository, ChargePointRepository>();
            services.AddScoped<IModuleRepository, ModuleRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddSingleton<ChargePointService>();
            services.AddSingleton<VehicleService>();

            services.AddAutoMapper(typeof(Startup));

            services.AddModuleServices();
            services.AddHostedService<ChargeModesHostedService>();

            services.AddScoped<IChargeModesService, ChargeModesService>();
            services.AddScoped<StopChargeMode>();
            services.AddScoped<DirectChargeMode>();
            services.AddScoped<ExcessChargeMode>();

            services.AddHttpClient<HomeAssistantHttpClient>();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, OpenHomeEnergyManagerDbContext dbContext)
        {
            dbContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenHomeEnergyManager.Api v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
