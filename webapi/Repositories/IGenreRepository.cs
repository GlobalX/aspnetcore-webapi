using System.Threading.Tasks;
using webapi.Models;

namespace webapi.Repositories
{
    public interface IGenreRepository : ISequenceRepository<Genre>
    {
        Task<Genre> GetByName(string name);
    }
}