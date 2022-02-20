using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using ZO1.Identity.WebApp.Cores.Contexts;
using ZO1.Identity.WebApp.Cores.Models.Models;
using ZO1.Identity.WebApp.Services;
using ZO1.Identity.WebApp.Settings;

namespace ZO1.Identity.WebApp
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
            // Config facebook
            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = Configuration["FacebookAppId"];
                options.AppSecret = Configuration["FacebookAppSecret"];
            });

            // Inject authentication handler
            services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", cookieOptions =>
            {
                cookieOptions.Cookie.Name = "MyCookieAuth";
                cookieOptions.LoginPath = "/Account/Login";
                cookieOptions.AccessDeniedPath = "/Account/Login";
                cookieOptions.ExpireTimeSpan = TimeSpan.FromDays(3);
            });

            // Config DbContext with connection strings
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("AppIdentityCore"));
            });

            // Use Identity
            services.AddIdentity<User, IdentityRole>(options =>
                    {
                        options.Password.RequiredLength = 8;
                        options.Password.RequireUppercase = true;
                        options.Password.RequireLowercase = true;

                        options.Lockout.MaxFailedAccessAttempts = 5;
                        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMilliseconds(15);

                        options.User.RequireUniqueEmail = true;
                        options.SignIn.RequireConfirmedEmail = true;
                    })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Config cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.LoginPath = "/Account/Login";
            });


            // Config Smtp
            /* We telling asp.net core when this class required of any services class
             * If need to go to this section in appSetting.json to grab the information
             * Then map with SmtpSetting class
             */
            services.Configure<SmtpSetting>(Configuration.GetSection("Smtp"));
            services.AddSingleton<IEmailServices, EmailServices>();


            // Use razor page
            services.AddRazorPages();
            services.AddControllers();


            // Config session
            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromHours(8);
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
                endpoints.MapControllers();
            });
        }
    }
}
