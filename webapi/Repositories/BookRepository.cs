using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
            const string sql = "SELECT book_Id as Id, book_number BookNumber, created_at as CreatedAt, title, year, author_id as AuthorId, tenant_id as TenantId FROM public.books;";

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
            const string sql = "SELECT book_Id as Id, book_number BookNumber, created_at as CreatedAt, title, year, author_id as AuthorId, tenant_id as TenantId FROM public.books WHERE book_id = @Id;";
            var parameters = new { Id = id };

            using (var connection = _createConnection())
            {
                connection.Open();

                var book = connection.Query<Book>(sql, parameters).FirstOrDefault();
                connection.Close();

                return book;
            }
        }

        public Guid Insert(Book entity)
        {
            const string sql = "INSERT INTO public.books(book_Id, book_number, tenant_id, title, year, created_at, author_id, genre_id)" +
                               "VALUES(@Id, nextval(@TenantBookSequence), @TenantId, @Title, @Year, current_timestamp, @AuthorId, @GenreId);";
            var newBookId = Guid.NewGuid();
            var parameters = new {  Id = newBookId, TenantId = entity.TenantId, 
                                    TenantBookSequence = "books_" + entity.TenantId.ToString().Replace("-", "") + "_book_id_seq", 
                                    Title = entity.Title, Year = entity.Year, AuthorId = entity.AuthorId, GenreId = entity.GenreId };

            using (var connection = _createConnection())
            {
                connection.Open();

                var book = connection.Execute(sql, parameters);
                connection.Close();
                return newBookId;
            }
        }

        public void Update(Book entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid bookId)
        {
            const string sql = "DELETE FROM public.books WHERE book_id = @Id;";
            var parameters = new { Id = bookId };

            using (var connection = _createConnection())
            {
                connection.Open();

                var book = connection.Execute(sql, parameters);
                connection.Close();

                return;
            }
        }

        public Task<IEnumerable<Book>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Book> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(Book entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Book entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}