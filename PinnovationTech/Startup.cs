using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PinnovationTech.Interface;
using PinnovationTech.Repository;
using System;

namespace PinnovationTech
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
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddSingleton(Configuration);
            services.AddSingleton<ILoginRepository, LoginRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddControllersWithViews();
            services.AddAuthentication(x =>
            {
                x.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(IdentityConstants.ApplicationScheme);

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Login/Index");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseResponseCompression();
            }

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Login";
                    await next();
                }
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // LOGIN ROUTE
                endpoints.MapControllerRoute(name: "LogIn", pattern: "LogIn", defaults: new { controller = "Login", action = "Index" });
                endpoints.MapControllerRoute(name: "LogOut", pattern: "LogOut", defaults: new { controller = "Login", action = "LogOut" });
                endpoints.MapControllerRoute(name: "Register", pattern: "Register", defaults: new { controller = "Login", action = "RegisterUser" });
                endpoints.MapControllerRoute(name: "GetCities", pattern: "GetCities", defaults: new { controller = "Login", action = "GetCities" });
                endpoints.MapControllerRoute(name: "LogOut", pattern: "LogOut", defaults: new { controller = "Login", action = "LogOut" });
                endpoints.MapControllerRoute(name: "View", pattern: "View", defaults: new { controller = "Login", action = "GetDetail" });
                endpoints.MapControllerRoute(name: "Edit", pattern: "Edit", defaults: new { controller = "Login", action = "EditUser" });

                // HOME ROUTE
                endpoints.MapControllerRoute(name: "Home", pattern: "Home", defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
