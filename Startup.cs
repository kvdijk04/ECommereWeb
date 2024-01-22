//using AspNetCore.SEOHelper;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnlineMarket.Helper;
using OnlineMarket.Models;
using Stripe;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Web;
namespace WebApplication1
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
            var stringConnectDb = Configuration.GetConnectionString("dbOnlineMarket");
            services.AddDbContext<OnlineShopContext>(options => options.UseSqlServer(stringConnectDb));
            services.AddSession();
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(p =>
            {
                p.LoginPath = "/dang-nhap.html";
                p.AccessDeniedPath = "/";
            });




            services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });
            services.AddMvc().AddNewtonsoftJson();
            services.AddRazorPages(options =>
            {
                options.Conventions.AddPageRoute("/Sitemap", "Sitemap.xml");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
           
     
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //d?c file robot
            //app.UseRobotsTxt(env);
            //app.UseStaticFiles();
            //
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            //StripeConfiguration.ApiKey = "sk_test_51K7kqoIcuazq9BbEJQ3WZ47tLGOhrpPViTqSMavf19a8sKytYbqFa2OgbWQfp7NztJoLNRiOEDp3dAsbYlk9jEtj00qfh72RIV";
            StripeConfiguration.ApiKey = "sk_test_51M1PrLE0sjCO0f3ufSXlal5XlQmuXm3x0xYvkyvOxyTsZftM9t6kTzIvj2hPP7O1OeAPphQ391YT4SuaJaFKXz6P00lxB8Fq3F";
            //app.UseXMLSitemap(env.ContentRootPath);
            //app.UseRobotsTxt(env.ContentRootPath);
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                 name: "areas",
                 pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
