using CMapTest.Auth;
using CMapTest.Config;
using CMapTest.Data;
using CMapTest.Reports;
using CMapTest.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace CMapTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o =>
            {
                o.LoginPath = new PathString("/Index"); // caused redirect to login page
                AuthOptions authOpt = builder.Configuration.GetRequiredSection("AuthOptions").Get<AuthOptions>()!;
                o.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
                o.ExpireTimeSpan = authOpt.CookieExpiry;
                o.Validate();
            });
            builder.Services.AddAuthorization(o =>
            {
                o.AddPolicy("Admin", p =>
                {
                    p.RequireClaim(ClaimTypes.UserRole, UserRole.Admin.ToString());
                });
            });
            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.Configure<AuthOptions>(builder.Configuration.GetRequiredSection("AuthOptions"));
            builder.Services.AddSingleton<DataLayer>();
#if DEBUG
            builder.Services.AddSingleton<IDataLayer>(s => s.GetRequiredService<DataLayer>());
#endif
            builder.Services.AddSingleton<IAuthDataLayer>(s => s.GetRequiredService<DataLayer>());
            builder.Services.AddSingleton<IUserDataLayer>(s => s.GetRequiredService<DataLayer>());
            builder.Services.AddSingleton<IProjectsDataLayer>(s => s.GetRequiredService<DataLayer>());
            builder.Services.AddSingleton<IEntriesDataLayer>(s => s.GetRequiredService<DataLayer>());

            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IReportGenerator, ReportGenerator>();

            var app = builder.Build();

            if (builder.Environment.IsDevelopment())
            {
                // seeding
#if DEBUG
                IDataLayer data = app.Services.GetRequiredService<IDataLayer>();
                data.Seed();
#endif
            }
            // Configure the HTTP request pipeline.
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

            app.MapRazorPages();

            app.Run();
        }
    }
}
