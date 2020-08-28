using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using webapi.Models;

namespace webapi.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        public Task<Author> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Author> GetAll()
        {
            throw new NotImplementedException();
        }

        public Author GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Insert(Author entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Author entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}