using AutoMapper;
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
using OpenHomeEnergyManager.Domain.Services.DataHistorizationServices;
using OpenHomeEnergyManager.Domain.Services.VehicleServices;
using OpenHomeEnergyManager.Infrastructure.ChargeModes;
using OpenHomeEnergyManager.Infrastructure.DataHistorization;
using OpenHomeEnergyManager.Infrastructure.EntityFramework;
using OpenHomeEnergyManager.Infrastructure.EntityFramework.Repositories;
using OpenHomeEnergyManager.Infrastructure.Modules.DependencyInjection;
using OpenHomeEnergyManager.Infrastructure.Modules.HomeAssistant;
using OpenHomeEnergyManager.Infrastructure.Modules.Tesla.Authentication;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OpenHomeEnergyManager.Api", Version = "v1" });
            });

            services.AddDbContext<OpenHomeEnergyManagerDbContext>(options => options.UseSqlite("Data Source=./data/OpenHomeEnergyManager.db"));

            services.AddScoped<IChargePointRepository, ChargePointRepository>();
            services.AddScoped<IModuleRepository, ModuleRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddSingleton<ChargePointService>();
            services.AddSingleton<VehicleService>();

            services.AddAutoMapper(typeof(Startup));

            services.AddModuleServices();
            //services.AddHostedService<ChargeModesHostedService>();
            //services.AddHostedService<DataHistorizationHostedService>();

            services.AddSingleton<DataHistorizationService>();

            services.AddScoped<IChargeModesService, ChargeModesService>();
            services.AddScoped<StopChargeMode>();
            services.AddScoped<DirectChargeMode>();
            services.AddScoped<ExcessChargeMode>();

            services.AddScoped<DirectTargetChargeMode>();
            services.AddScoped<PlannedTargetChargeMode>();

            //services.AddHttpClient<HomeAssistantHttpClient>();
            //services.AddHttpClient<Infrastructure.Modules.Tesla.Authentication.LoginClient>()
            //    .ConfigurePrimaryHttpMessageHandler(() =>
            //    {
            //        return new HttpClientHandler()
            //        {
            //            AllowAutoRedirect = false
            //        };
            //    });

            //services.AddHttpClient<TeslaClient>(c => c.BaseAddress = new Uri("https://owner-api.teslamotors.com/api/1/"))
            //    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            //    .AddPolicyHandler(HttpPolicyExtensions
            //                        .HandleTransientHttpError()
            //                        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.RequestTimeout ||
            //                                         msg.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            //                        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
            //                        );


            //services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);

        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, OpenHomeEnergyManagerDbContext dbContext)
        {
            var folderPath = Path.Combine(env.ContentRootPath, "data", "images");
            if (!Directory.Exists(folderPath)) { Directory.CreateDirectory(folderPath); }

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
