using System;
using AudioCity.Areas.Identity.Data;
using AudioCity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(AudioCity.Areas.Identity.IdentityHostingStartup))]
namespace AudioCity.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AudioCityContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AudioCityContextConnection")));

                services.AddDefaultIdentity<AudioCityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()    
                .AddEntityFrameworkStores<AudioCityContext>();
            });
        }
    }
}