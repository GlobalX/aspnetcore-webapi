using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using webapi.Repositories;

namespace webapi.Infrastructure
{
    public static class ExtensionMethods
    {
        public static IServiceCollection UseMultiTenancyPostgresInterceptor(this IServiceCollection services,
            IConfiguration configuration)
            => services.UseEFInterceptor<MultiTenancyDbCommandInterceptor>(configuration);

        private static IServiceCollection UseEFInterceptor<T>(this IServiceCollection services,
            IConfiguration configuration)
            where T : class, IInterceptor
        {
            return services
                .AddScoped(serviceProvider =>
                {
                    var tenant = serviceProvider.GetRequiredService<TenantInfo>();

                    var efServices = new ServiceCollection();
                    efServices.AddEntityFrameworkNpgsql();
                    efServices.AddScoped(s =>
                        serviceProvider
                            .GetRequiredService<TenantInfo
                            >()); // Allows DI for tenant info, set by parent pipeline via middleware
                    efServices.AddScoped<IInterceptor, T>(); // Adds the interceptor

                    var connectionString =
                        "Host=192.168.1.115; Database=tenancytest; Username=appuser; Password=Welcome1";

                    return new DbContextOptionsBuilder<DatabaseContext>()
                        .UseInternalServiceProvider(efServices.BuildServiceProvider())
                        .UseNpgsql(connectionString)
                        .Options;
                })
                .AddScoped(s => new DatabaseContext(s.GetRequiredService<DbContextOptions<DatabaseContext>>()));
        }
    }
}