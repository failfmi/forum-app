using System.Linq;
using Forum.Data.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Forum.Data.Common
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ForumContext context;
        private readonly DbSet<T> set;

        public Repository(ForumContext context)
        {
            this.context = context;
            this.set = context.Set<T>();
        }

        public IQueryable<T> Query()
        {
            return this.set;
        }
    }
}
