using Npgsql;

namespace webapi.Infrastructure
{
    public class MultiTenantNgpSqlConnection : MultiTenantDbConnection
    {
        public MultiTenantNgpSqlConnection(string connection, TenantInfo tenantInfo) : base(tenantInfo)
        {
            Connection = new NpgsqlConnection(connection);
        }
    }
}