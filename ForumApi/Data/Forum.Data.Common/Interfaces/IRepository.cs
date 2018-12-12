using System.Linq;

namespace Forum.Data.Common.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        IQueryable<T> Query();
    }
}
