using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using ZO1.Identity.UnderTheHood.Authorizations;

namespace ZO1.Identity.UnderTheHood
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
            // inject authentication handler
            services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", cookieOptions =>
            {
                cookieOptions.Cookie.Name = "MyCookieAuth";
                cookieOptions.LoginPath = "/Account/Login";
                cookieOptions.AccessDeniedPath = "/Account/AccessDenied";
                cookieOptions.ExpireTimeSpan = TimeSpan.FromDays(3);
            });

            // simple custom with policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", 
                    policy => policy.RequireClaim("Admin"));

                options.AddPolicy("MustBelongToHRDepartment",
                    policy => policy.RequireClaim("Department", "HR"));

                options.AddPolicy("HRManagerOnly", policy => policy
                    .RequireClaim("Department", "HR")
                    .RequireClaim("Manager")
                    .Requirements.Add(new HrManagerProbationRequirement(3)));
            });

            services.AddSingleton<IAuthorizationHandler, 
                HrManagerProbationRequirementHandler>();

            services.AddRazorPages();

            //configs api HTTP Client Factory
            services.AddHttpClient("OurWebAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:44375/");
            });

            //add session
            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.IdleTimeout = TimeSpan.FromHours(8);
                options.Cookie.IsEssential = true;
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
