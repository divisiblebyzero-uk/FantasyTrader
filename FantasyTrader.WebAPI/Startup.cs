using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FantasyTrader.WebAPI.data;
using FantasyTrader.WebAPI.HubConfig;
using FantasyTrader.WebAPI.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Okta.AspNetCore;
using IdentityRole = Microsoft.AspNetCore.Identity.IdentityRole;
using IdentityUser = Microsoft.AspNetCore.Identity.IdentityUser;

namespace FantasyTrader.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<FantasyTraderDataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FantasyTraderSqlServerLocalDb")));
                //options.UseInMemoryDatabase("FantasyTrader"));
            services.AddScoped<DbInitialiser>();
            
            services.AddSingleton<FantasyMarketPriceSource>();
            services.AddSignalR().AddMessagePackProtocol().AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });


            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = OktaDefaults.ApiAuthenticationScheme;
                    options.DefaultChallengeScheme = OktaDefaults.ApiAuthenticationScheme;
                    options.DefaultSignInScheme = OktaDefaults.ApiAuthenticationScheme;
                })
                .AddOktaWebApi(new OktaWebApiOptions()
                {
                    OktaDomain = "https://dev-648496.okta.com"
                });
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.Converters.Add(new StringEnumConverter()));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<OrderHub>("/order");
                endpoints.MapHub<PriceHub>("/price");
            });
        }
    }
}
