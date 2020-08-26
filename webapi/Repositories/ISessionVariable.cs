using System.Data;
using Npgsql;
using webapi.Infrastructure;

namespace webapi.Repositories
{
    public interface ISessionVariable
    {
        void SetTenantIdIntoDatabaseSessionVariable(NpgsqlCommand npgsqlCommand, TenantInfo tenantInfo, IDbConnection connection);
    }
}