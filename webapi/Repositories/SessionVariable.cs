using System.Data;
using Npgsql;
using webapi.Infrastructure;

namespace webapi.Repositories
{
    public class SessionVariable : ISessionVariable
    {
        public void SetTenantIdIntoDatabaseSessionVariable(NpgsqlCommand npgsqlCommand, TenantInfo tenantInfo, IDbConnection connection)
        {
            //TODO: Use the existing command rather than round-tripping again
            var tenantCommand  = new NpgsqlCommand($"SET app.current_tenant = '{tenantInfo.Id}';", (NpgsqlConnection) connection)
            {
                CommandType = CommandType.Text
            };
            tenantCommand.ExecuteNonQuery();
        }
    }
}