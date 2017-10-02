using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public interface IGenericRepository<T, K> where T : class
    {
        IQueryable<T> GetAll();

        Task<T> GetAsync(int id);

        T Get(K BusinessKey);

        void Add(T entity);

        void Delete(T entity);

        void Delete(int id);

        Task<bool> SaveAsync();

        void Update(T entity);
    }
}