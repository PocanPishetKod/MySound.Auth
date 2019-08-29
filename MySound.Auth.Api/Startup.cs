using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySound.Auth.Api.ServiceConfiguration;

namespace MySound.Auth 
{
    public class Startup 
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment) 
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) 
        {
            services.AddServices()
                .AddEfCore(Configuration, Environment)
                .AddAuthentication(Configuration, Environment)
                .AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) 
        {
            if (!env.EnvironmentName.Equals("debug", StringComparison.CurrentCultureIgnoreCase))
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions()
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }

            if (env.IsDevelopment() || env.EnvironmentName.Equals("debug", StringComparison.CurrentCultureIgnoreCase))
            {
                app.UseDeveloperExceptionPage();
            }

            if (!env.EnvironmentName.Equals("debug", StringComparison.CurrentCultureIgnoreCase))
            {
                app.UseHttpsRedirection();
                app.UseHsts();
            }
            
            app.UseIdentityServer();
            app.UseMvc();
        }
    }
}