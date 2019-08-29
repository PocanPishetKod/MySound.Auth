using Microsoft.Extensions.DependencyInjection;
using MySound.Auth.IdentityServer;

namespace MySound.Auth.Api.ServiceConfiguration
{
    public static class ServicesConfiguration
    {
        /// <summary>
        /// Добавляет все сервисы, используемые DI
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<Config>();
            services.AddTransient<DbInitializer>();

            return services;
        }
    }
}