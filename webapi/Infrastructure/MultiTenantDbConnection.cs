using System.Data;
using System.Data.Common;

namespace webapi.Infrastructure
{
    public abstract class MultiTenantDbConnection : DbConnection
    {
        private readonly TenantInfo _tenantInfo;

        private bool _disposed;
        protected DbConnection Connection;

        protected MultiTenantDbConnection(TenantInfo tenantInfo)
        {
            _tenantInfo = tenantInfo;
        }

        public override string ConnectionString
        {
            get => Connection.ConnectionString;
            set => Connection.ConnectionString = value;
        }

        public override string Database => Connection.Database;


        public override string DataSource => Connection.DataSource;


        public override string ServerVersion => Connection.ServerVersion;


        public override ConnectionState State => Connection.State;

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return Connection.BeginTransaction();
        }

        public override void ChangeDatabase(string databaseName)
        {
            Connection.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            Connection.Close();
        }

        public override void Open()
        {
            Connection.Open();
        }

        protected override DbCommand CreateDbCommand()
        {
            return new MultiTenantDbCommand(Connection.CreateCommand(), _tenantInfo);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // No managed resources to release.
            }

            // Release unmanaged resources.
            Connection?.Dispose();
            Connection = null;
            // Do not release logger.  Its lifetime is controlled by caller.
            _disposed = true;
        }
    }
}