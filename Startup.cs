using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using slms2asp.Database;
using System.Text;

namespace slms2asp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private AppDbContext Db { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            using (Db = new AppDbContext(configuration))
            {
                Db.Database.EnsureCreated();
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the Angular files will be served from this directory
            services
                .AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services
                .AddSingleton<IConfiguration>(Configuration);

            services
                .AddDbContext<AppDbContext>();

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opt => {
                    //opt.SessionStore =
                    opt.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = async context =>
                        {
                            context.Response.StatusCode = 401;
                            var errorData = Encoding.UTF8.GetBytes("{\"code\": 401,\"message\": \"unauthorized\"}");
                            await context.Response.Body.WriteAsync(errorData);
                        }
                    };

                    opt.LogoutPath = opt.LoginPath;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app
                .UseStaticFiles()
                .UseSpaStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
