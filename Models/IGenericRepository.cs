using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public interface IGenericRepository<T, K> where T : class
    {
        IQueryable<T> GetAll();

        Task<T> GetAsync(int id);

        T Get(K BusinessKey);

        T GetWithoutTracking(K BusinessKey);

        void Add(T entity, bool commit = true);

        void Delete(T entity, bool commit = true);

        void Delete(int id, bool commit = true);

        Task<bool> SaveAsync();

        void Update(T entity, bool commit = true);
    }
}