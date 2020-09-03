using System;
using System.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using webapi.Repositories;
using webapi.Infrastructure;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Scrutor;

namespace webapi
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
            services.AddScoped<TenantInfo>();

            services.Scan(scan => scan
                .FromCallingAssembly()
                .AddClasses(classes => classes.Where(type => type.Namespace.EndsWith("Repositories")))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.AddScoped<Func<IDbConnection>>(c =>
            {
                return () =>
                {
                    var conn = new MultiTenantNgpSqlConnection(
                                    string.Format(Configuration.GetConnectionString("TenancyTest"), Configuration["DBHost"]), 
                                    c.GetRequiredService<TenantInfo>());
                    return conn;
                };
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMiddleware<TenantInfoMiddleware>();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
