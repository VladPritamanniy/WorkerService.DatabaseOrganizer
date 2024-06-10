using Microsoft.EntityFrameworkCore;

namespace DatabaseOrganizer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
