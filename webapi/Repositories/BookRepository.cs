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
        private readonly Func<IDbConnection> _createConnection;

        public BookRepository(Func<IDbConnection> createConnection)
        {
            _createConnection = createConnection;
        }

        public IEnumerable<Book> GetAll()
        {
            const string sql = "SELECT \"Id\", \"CreatedAt\", \"Title\", \"Year\", \"AuthorId\", \"TenantId\" FROM public.books;";

            using (var connection = _createConnection())
            {
                connection.Open();

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
            const string sql = "INSERT INTO public.books(\"Id\", \"TenantId\", \"Title\", \"Year\", \"CreatedAt\", \"AuthorId\")" +
                               "VALUES(@Id, @TenantId, @Title, @Year, current_timestamp, @AuthorId);";
            var parameters = new { Id = entity.Id, TenantId = entity.TenantId, Title = entity.Title, Year = entity.Year, AuthorId = entity.AuthorId };

            using (var connection = _createConnection())
            {
                connection.Open();

                var book = connection.Execute(sql, parameters);
                connection.Close();
            }
        }

        public void Update(Book entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            const string sql = "DELETE FROM public.books WHERE \"Id\" = @Id;";
            var parameters = new { Id = id };

            using (var connection = _createConnection())
            {
                connection.Open();

                var book = connection.Execute(sql, parameters);
                connection.Close();

                return;
            }
        }
    }
}