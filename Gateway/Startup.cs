using DatabaseAccessLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Gateway.Services;
using Gateway.Services.Authorization;
using Gateway.Services.Hashing;
using System;
using System.Threading.Tasks;
using CacheAccessLayer;
using CacheAccessLayer.Modules;

namespace Gateway
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
            services.AddControllers();

            services.AddDbContext<DatabaseContext>(options =>
            {
                var connectionStirng = default(string);
                if (!string.IsNullOrEmpty(connectionStirng = Configuration.GetConnectionString("postgres")))
                    options.UseNpgsql(connectionStirng);
                else if (!string.IsNullOrEmpty(connectionStirng = Configuration.GetConnectionString("mysql")))
                    options.UseMySQL(connectionStirng);
                else
                    throw new ArgumentException("unsupported database connection string provided");
            });

            services
                .AddScoped<IDatabaseAccess, DatabaseAccess>()
                .AddSingleton<ICacheAccess, RedisCacheModule>()
                .AddSingleton<IPasswordHashingService, Argon2HashingService>()
                .AddSingleton<IHashingService, Sha1HashingService>()
                .AddSingleton<IAuthorizationService, JwtAuthorizationService>()
                .AddScoped<InitializationService>()
                ;

            services.AddSwaggerGen(c => 
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "vctr", Version = "v1" }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway v1"));
                app.UseCors(options => options
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => 
                endpoints.MapControllers());

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var initializationService = scope.ServiceProvider.GetService<InitializationService>();
                var logger = scope.ServiceProvider.GetService<ILogger<Startup>>();
                Task.Run(async () =>
                {
                    try
                    {
                        logger.LogInformation("Running initialization routine...");
                        await initializationService.InitializationRoutine();
                    } catch (Exception e)
                    {
                        logger.LogError($"Initialization Routine Failed: {e.Message}");
                    }
                }).Wait();
            }

        }
    }
}
