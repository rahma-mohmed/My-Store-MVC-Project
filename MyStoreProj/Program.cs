using Microsoft.EntityFrameworkCore;
using MyStore.web;
using mystore.DataAccess;
using mystore.Entities.Repositories;
using mystore.DataAccess.Implementation;
using Microsoft.AspNetCore.Identity;
using mystore.Utilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;

namespace MyStore.web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<mystore.DataAccess.ApplicationDbContext>(options => options.UseSqlServer(
                builder.Configuration.GetConnectionString("DafaultConnection")
                ));
            //
            builder.Services.Configure<StripeData>(builder.Configuration.GetSection("stripe"));

			//lock when enter wrong user name - pass , login
			builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20))
                .AddDefaultTokenProviders().AddDefaultUI()
                .AddEntityFrameworkStores<mystore.DataAccess.ApplicationDbContext>();

            builder.Services.AddSingleton<IEmailSender,EmailSender>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:SecretKey").Get<string>();

			app.UseAuthentication();
            app.UseAuthorization();
            app.MapRazorPages();  
            
            //app.MapControllerRoute(
            //    name: "Admin",
            //    pattern: "{area=Admin}/{controller=Category}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
    }
}
