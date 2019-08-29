using System;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySound.Auth.EF.Context;
using MySound.Auth.Models.Domain;

namespace MySound.Auth.Api.ServiceConfiguration 
{
    public static class EfConfiguration 
    {
        /// <summary>
        /// Настраивает EntityFrameworkCore и Identity
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddEfCore(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment environment) 
        {
            services.AddDbContext<AuthDbContext>(options =>
            {
                if (environment.EnvironmentName.Equals("debug", StringComparison.CurrentCultureIgnoreCase))
                {
                    options.UseSqlServer(configuration.GetConnectionString("MySound.Auth.Connection.Debug"));
                }
                else
                {
                    options.UseSqlServer(configuration.GetConnectionString("MySound.Auth.Connection"));
                }
            });
            
            services.AddIdentity<Account, IdentityRole>(options => 
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 2;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}