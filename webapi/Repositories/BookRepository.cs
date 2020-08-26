using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Npgsql;
using webapi.Infrastructure;
using webapi.Models;

namespace webapi.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ISessionVariable _sessionVariable;
        private readonly Func<IDbConnection> _createConnection;
        private readonly TenantInfo _tenantInfo;

        public BookRepository(ISessionVariable sessionVariable, Func<IDbConnection> createConnection, TenantInfo tenantInfo)
        {
            _sessionVariable = sessionVariable;
            _createConnection = createConnection;
            _tenantInfo = tenantInfo;
        }

        public IEnumerable<Book> GetAll()
        {
            const string sql = "SELECT \"Id\", \"CreatedAt\", \"Title\", \"Year\", \"AuthorId\", \"TenantId\" FROM public.books;";

            using (var connection = _createConnection())
            {
                connection.Open();

                _sessionVariable.SetTenantIdIntoDatabaseSessionVariable(new NpgsqlCommand(), _tenantInfo, connection);

                //get trust accounts
                var books = connection.Query<Book>(sql);
                connection.Close();

                return books;
            }
        }

        public Book GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Insert(Book entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Book entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}