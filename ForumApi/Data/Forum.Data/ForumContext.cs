using Forum.Data.Models;
using Forum.Data.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Forum.Data
{
    public class ForumContext : IdentityDbContext<User>
    {
        public ForumContext(DbContextOptions<ForumContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder
                .Entity<User>()
                .Property(u => u.IsActive)
                .HasDefaultValue(true);

            builder
                .Entity<User>()
                .Property(u => u.IsLogged)
                .HasDefaultValue(false);
        }
    }
}
