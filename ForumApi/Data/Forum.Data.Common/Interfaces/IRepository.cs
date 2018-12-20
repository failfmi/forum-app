using System.Linq;
using System.Threading.Tasks;

namespace Forum.Data.Common.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        IQueryable<T> Query();

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task<int> SaveChangesAsync();
    }
}
