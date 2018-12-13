using System;
using System.Linq;
using System.Threading.Tasks;
using Forum.Data.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Forum.Data.Common
{
    public class Repository<T> : IRepository<T>, IDisposable where T : class
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

        public Task AddAsync(T entity)
        {
            return this.set.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            this.set.Remove(entity);
        }

        public Task<int> SaveChangesAsync()
        {
            return this.context.SaveChangesAsync();
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
