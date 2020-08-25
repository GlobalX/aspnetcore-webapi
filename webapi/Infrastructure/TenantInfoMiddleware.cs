using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace webapi.Infrastructure
{
    public class TenantInfoMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //TODO: get from jwt
            var tenantInfo = context.RequestServices.GetRequiredService<TenantInfo>();
            tenantInfo.Id = new Guid("a5bab93a-1f7d-4fae-8bf6-ad7f2d6838fe");

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}