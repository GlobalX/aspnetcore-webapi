using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace webapi.Infrastructure
{
    public class MultiTenancyDbCommandInterceptor : DbCommandInterceptor
    {
        private readonly TenantInfo _tenantInfo;

        public MultiTenancyDbCommandInterceptor(TenantInfo tenantInfo)
        {
            _tenantInfo = tenantInfo;
        }

        public override Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            command.CommandText = $"SET app.current_tenant = '{_tenantInfo.Id}'; {command.CommandText}";

            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override Task<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            command.CommandText = $"SET app.current_tenant = '{_tenantInfo.Id}'; {command.CommandText}";
            
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override Task<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            command.CommandText = $"SET app.current_tenant = '{_tenantInfo.Id}'; {command.CommandText}";

            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }
    }
}