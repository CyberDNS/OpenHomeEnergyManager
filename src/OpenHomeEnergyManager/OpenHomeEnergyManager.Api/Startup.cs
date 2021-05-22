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
using OpenHomeEnergyManager.Domain.Services.ChargePoint;
using OpenHomeEnergyManager.Infrastructure.EntityFramework;
using OpenHomeEnergyManager.Infrastructure.EntityFramework.Repositories;
using OpenHomeEnergyManager.Infrastructure.Modules.DependencyInjection;
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
            services.AddSingleton<ChargePointService>();

            services.AddAutoMapper(typeof(Startup));

            services.AddModuleServices();
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
