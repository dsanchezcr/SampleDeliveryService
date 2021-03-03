using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SampleDeliveryService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleDeliveryService.Services;

namespace SampleDeliveryService
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
            services.AddCors(options =>
            {
                options.AddPolicy("LocalAzure",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost",
                                            "http://<App Service URL>")
                               .WithMethods("GET");
                    });
            });
            services.AddTransient<TokenAuthorizationProvider>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest).AddRazorRuntimeCompilation();  
            services.AddSingleton<ICosmosDbService>(InitializeCosmosClientInstanceAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
            services.AddAuthentication("Bearer").AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    IssuerSigningKey = TokenAuthorizationProvider.CreateSecurityKey(),
                    ValidIssuer = TokenAuthorizationProvider.Issuer,
                    ValidAudience = TokenAuthorizationProvider.Audience
                };
            });
            services.AddAuthorization(options =>
            {
                AuthorizationPolicyBuilder builder = new AuthorizationPolicyBuilder("Bearer");
                options.AddPolicy("SessionToken", builder.RequireAuthenticatedUser().Build());
            });
        }

        private static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("ContainerName").Value;
            string account = configurationSection.GetSection("Account").Value;
            string key = configurationSection.GetSection("Key").Value;
            Microsoft.Azure.Cosmos.CosmosClient client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            CosmosDbService cosmosDbService = new CosmosDbService(client, databaseName, containerName);
            Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return cosmosDbService;
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Orders/Error");
                app.UseHsts();
            }           

            app.UseHttpsRedirection();
            app.UseStaticFiles();            
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Orders}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}