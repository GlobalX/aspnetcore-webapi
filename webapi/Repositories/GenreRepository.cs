using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using webapi.Models;
using Dapper;

namespace webapi.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly Func<IDbConnection> _createConnection;

        public GenreRepository(Func<IDbConnection> createConnection)
        {
            _createConnection = createConnection;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Genre> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Genre>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Genre GetById(int id)
        {
            const string sql = "SELECT genre_id as Id, name FROM public.genres WHERE genre_id = @Id;";
            var parameters = new { Id = id };

            using (var connection = _createConnection())
            {
                connection.Open();

                var genre = connection.Query<Genre>(sql, parameters).FirstOrDefault();
                connection.Close();

                return genre;
            }
        }

        public Task<Genre> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Genre> GetByName(string name)
        {
            const string sql = "SELECT genre_id as Id, name FROM public.genres WHERE name = @Name;";
            var parameters = new { Name = name };

            using (var connection = _createConnection())
            {
                connection.Open();

                var genre = (await connection.QueryAsync<Genre>(sql, parameters)).FirstOrDefault();
                connection.Close();

                return genre;
            }
        }

        public void Insert(Genre entity)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(Genre entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Genre entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Genre entity)
        {
            throw new NotImplementedException();
        }
    }
}