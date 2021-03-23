using Api.Classes;
using Betkeeper;
using Betkeeper.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ApiCore
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

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
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder
                        .WithOrigins(
                            "http://localhost:3001",
                            "http://localhost:5000",
                            "http://betkeeper.azurewebsites.net",
                            "https://betkeeper.azurewebsites.net")
                        .WithHeaders(HeaderNames.Authorization, HeaderNames.ContentType)
                        .WithMethods("POST", "PUT", "DELETE", "GET");
                    });
            });

            services.AddControllers();
            services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiCore", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiCore v1"));
            }

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(context =>
                {
                    var uri = context.Request.Path.ToUriComponent();

                    new ErrorLogger().LogError(context.Features.Get<IExceptionHandlerFeature>(), uri);

                    return null;
                });
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseEndpoints(endpoints =>
            {
                endpoints
                    .MapControllers()
                    .RequireCors(MyAllowSpecificOrigins);
            });

            var devSecretsPath = Configuration.GetValue<string>("DevSecretsPath");
            var secretsObject = File.Exists(devSecretsPath)
                ? JsonConvert.DeserializeObject(File.ReadAllText(devSecretsPath)) as dynamic
                : null;
            // Set database connection
            Settings.ConnectionString = Configuration.GetConnectionString("Sql")
                ?? secretsObject["ConnectionString"]?.ToString();
            Settings.SecretKey = Configuration.GetValue<string>("SecretKey")
                ?? secretsObject["SecretKey"]?.ToString();

            Settings.LogErrors = Configuration.GetValue<bool>("LogErrors");

            if (string.IsNullOrEmpty(Settings.ConnectionString))
            {
                throw new Exception("Connection string was not given");
            }
            if (string.IsNullOrEmpty(Settings.SecretKey))
            {
                throw new Exception("Secret key was not given");
            }

            Settings.InitializeOptionsBuilderService();
            PageService.InitializePageTypes();
        }
    }
}
