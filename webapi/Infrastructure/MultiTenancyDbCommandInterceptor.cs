using System;
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

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            command.CommandText = $"SET app.current_tenant = '{_tenantInfo.Id}'; {command.CommandText}";
            string query = command.CommandText;

            foreach (DbParameter p in command.Parameters)
            {
                query = query.Replace(p.ParameterName, p.Value.ToString());
            }
            Console.WriteLine(query);

            return base.ReaderExecuting(command, eventData, result);
        }

        public override Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            command.CommandText = $"SET app.current_tenant = '{_tenantInfo.Id}'; {command.CommandText}";
            string query = command.CommandText;

            foreach (DbParameter p in command.Parameters)
            {
                query = query.Replace(p.ParameterName, p.Value.ToString());
            }
            Console.WriteLine(query);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override Task<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            command.CommandText = $"SET app.current_tenant = '{_tenantInfo.Id}'; {command.CommandText}";
            string query = command.CommandText;

            foreach (DbParameter p in command.Parameters)
            {
                query = query.Replace(p.ParameterName, p.Value.ToString());
            }
            Console.WriteLine(query);
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override Task<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            command.CommandText = $"SET app.current_tenant = '{_tenantInfo.Id}'; {command.CommandText}";
            Console.WriteLine(command.CommandText);

            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            command.CommandText = $"SET app.current_tenant = '{_tenantInfo.Id}'; {command.CommandText}";
            string query = command.CommandText;

            foreach (DbParameter p in command.Parameters)
            {
                query = query.Replace(p.ParameterName, p.Value.ToString());
            }
            Console.WriteLine(query);
            return base.ScalarExecuting(command, eventData, result);
        }

        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            command.CommandText = $"SET app.current_tenant = '{_tenantInfo.Id}'; {command.CommandText}";

            string query = command.CommandText;

            foreach (DbParameter p in command.Parameters)
            {
                query = query.Replace(p.ParameterName, p.Value.ToString());
            }
            Console.WriteLine(query);
            return base.NonQueryExecuting(command, eventData, result);
        }
    }
}