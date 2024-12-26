using Microsoft.EntityFrameworkCore;
using TBA.Models;

namespace TBA.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public required DbSet<User> Users { get; set; }
    }
}
