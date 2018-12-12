using Forum.Data.Models;
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
    }
}
