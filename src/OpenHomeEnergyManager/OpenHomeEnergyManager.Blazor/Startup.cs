using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor;
using MudBlazor.Services;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients;
using OpenHomeEnergyManager.Blazor.Infrastructure.Mqtt.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Blazor
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
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddMudServices();

            services.AddHttpClient<ChargePointClient>(o => o.BaseAddress = new Uri($"{Configuration.GetValue<string>("HttpClients:Uris:OpenHomeEnergyManagerApi")}ChargePoints/"));
            services.AddHttpClient<ModuleServiceDefinitionClient>(o => o.BaseAddress = new Uri($"{Configuration.GetValue<string>("HttpClients:Uris:OpenHomeEnergyManagerApi")}ModuleServiceDefinitions/"));
            services.AddHttpClient<ModuleClient>(o => o.BaseAddress = new Uri($"{Configuration.GetValue<string>("HttpClients:Uris:OpenHomeEnergyManagerApi")}Modules/"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
