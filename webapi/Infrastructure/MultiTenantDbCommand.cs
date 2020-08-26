using System.Data;
using System.Data.Common;

namespace webapi.Infrastructure
{
    public class MultiTenantDbCommand : DbCommand
    {
        private DbCommand _command;
        private readonly TenantInfo _tenantInfo;
        private bool _disposed;


        public override string CommandText
        {
            get => $"SET app.current_tenant = '{_tenantInfo.Id}';{ _command.CommandText}";
            set => _command.CommandText = value;
        }


        public override int CommandTimeout
        {
            get => _command.CommandTimeout;
            set => _command.CommandTimeout = value;
        }


        public override CommandType CommandType
        {
            get => _command.CommandType;
            set => _command.CommandType = value;
        }


        public override UpdateRowSource UpdatedRowSource
        {
            get => _command.UpdatedRowSource;
            set => _command.UpdatedRowSource = value;
        }


        protected override DbConnection DbConnection
        {
            get => _command.Connection;
            set => _command.Connection = value;
        }


        protected override DbParameterCollection DbParameterCollection => _command.Parameters;


        protected override DbTransaction DbTransaction
        {
            get => _command.Transaction;
            set => _command.Transaction = value;
        }


        public override bool DesignTimeVisible
        {
            get => _command.DesignTimeVisible;
            set => _command.DesignTimeVisible = value;
        }
        

        public MultiTenantDbCommand(DbCommand command, TenantInfo tenantInfo)
        {
            _command = command;
            _tenantInfo = tenantInfo;
        }

        ~MultiTenantDbCommand() => Dispose(false);


        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // No managed resources to release.
            }
            // Release unmanaged resources.
            _command?.Dispose();
            _command = null;
            // Do not release logger.  Its lifetime is controlled by caller.
            _disposed = true;
        }


        public override void Cancel()
        {
            _command.Cancel();
        }


        public override int ExecuteNonQuery()
        {
            _command.CommandText = CommandText;
            var result = _command.ExecuteNonQuery();
            return result;
        }


        public override object ExecuteScalar()
        {
            _command.CommandText = CommandText;
            return _command.ExecuteScalar();
        }


        public override void Prepare()
        {
            _command.Prepare();
        }

        protected override DbParameter CreateDbParameter() => _command.CreateParameter();


        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            _command.CommandText = CommandText;
            return _command.ExecuteReader(behavior);
        }

    }
}