using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySound.Auth.Api.IdentityServer;
using MySound.Auth.EF.Context;
using MySound.Auth.Models.Domain;

namespace MySound.Auth.Api.ServiceConfiguration 
{
    public static class IdentityServerConfiguration 
    {
        /// <summary>
        /// Настраивает IdentityServer
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment environment) 
        {
            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddAspNetIdentity<User>()
                .AddConfigurationStore(options =>
                {
                    if (environment.EnvironmentName.Equals("debug", StringComparison.CurrentCultureIgnoreCase))
                    {
                        options.ConfigureDbContext = x => x.UseSqlServer(configuration.GetConnectionString("MySound.Auth.Connection.Debug"),
                        sql => sql.MigrationsAssembly(typeof(IdentityServerConfiguration).Assembly.GetName().Name));
                    }
                    else
                    {
                        options.ConfigureDbContext = x => x.UseSqlServer(configuration.GetConnectionString("MySound.Auth.Connection"),
                        sql => sql.MigrationsAssembly(typeof(IdentityServerConfiguration).Assembly.GetName().Name));
                    }
                })
                .AddOperationalStore(options => 
                {
                    if (environment.EnvironmentName.Equals("debug", StringComparison.CurrentCultureIgnoreCase))
                    {
                        options.ConfigureDbContext = x => x.UseSqlServer(configuration.GetConnectionString("MySound.Auth.Connection.Debug"),
                        sql => sql.MigrationsAssembly(typeof(IdentityServerConfiguration).Assembly.GetName().Name));
                    }
                    else
                    {
                        options.ConfigureDbContext = x => x.UseSqlServer(configuration.GetConnectionString("MySound.Auth.Connection"),
                        sql => sql.MigrationsAssembly(typeof(IdentityServerConfiguration).Assembly.GetName().Name));
                    }
                })
                .AddSigningCredential(Rsa.GenerateKeys());

            return services;
        }
    }
}