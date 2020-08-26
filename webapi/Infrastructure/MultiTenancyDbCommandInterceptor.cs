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

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            SetTenantOnQuery(command);
            return base.ReaderExecuting(command, eventData, result);
        }

        public override Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            SetTenantOnQuery(command);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override Task<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            SetTenantOnQuery(command);
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override Task<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            SetTenantOnQuery(command);
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            SetTenantOnQuery(command);
            return base.ScalarExecuting(command, eventData, result);
        }

        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            SetTenantOnQuery(command);
            return base.NonQueryExecuting(command, eventData, result);
        }

        private void SetTenantOnQuery(DbCommand command)
        {
            //TODO: Use the existing command rather than round-tripping again
            var tenantCommand  = new NpgsqlCommand($"SET app.current_tenant = '{_tenantInfo.Id}';", (NpgsqlConnection) command.Connection)
            {
                CommandType = CommandType.Text
            };
            tenantCommand.ExecuteNonQuery();

            var query = command.Parameters.Cast<DbParameter>().
                Aggregate(command.CommandText, (current, p) => current.Replace(p.ParameterName, p.Value.ToString()));

            Console.WriteLine(query);
        }
    }
}