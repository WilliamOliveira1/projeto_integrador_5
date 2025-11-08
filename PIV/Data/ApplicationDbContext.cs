using Microsoft.EntityFrameworkCore;
using PIV.Models;

namespace PIV.Data
{
    public class ApplicationDbContext : DbContext // Add inheritance from DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Weather> Weather { get; set; }
        public DbSet<SensorData> SensorData { get; set; }
    }
}
