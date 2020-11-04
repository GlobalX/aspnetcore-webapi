using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace webapi.Infrastructure
{
    public class MultiTenancyDbCommandInterceptor : DbCommandInterceptor
    {
        private readonly TenantInfo _tenantInfo;

        public MultiTenancyDbCommandInterceptor(TenantInfo tenantInfo)
        {
            _tenantInfo = tenantInfo;
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            var postgresCommand = command as Npgsql.NpgsqlCommand;

            return base.ReaderExecuted(command, eventData, result);
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            SetTenantOnQuery(ref command);
            return base.ReaderExecuting(command, eventData, result);
        }

        public override Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            SetTenantOnQuery(ref command);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override Task<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            SetTenantOnQuery(ref command);
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override Task<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            SetTenantOnQuery(ref command);
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            SetTenantOnQuery(ref command);
            return base.ScalarExecuting(command, eventData, result);
        }

        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            SetTenantOnQuery(ref command);
            return base.NonQueryExecuting(command, eventData, result);
        }

        private void SetTenantOnQuery(ref DbCommand command)
        {
            command.CommandText = $"SET app.current_tenant = '{_tenantInfo.Id}';" + command.CommandText;
        }
    }
}