using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using webapi.Models;

namespace webapi.Repositories
{
    public interface ISequenceRepository<T> where T : BaseSequenceEntity
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(int id);

        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task InsertAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}