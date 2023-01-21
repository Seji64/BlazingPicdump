using BlazingPicdump.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazingPicdump.DB
{
    public class PicdumpDbContext : DbContext
    {
        public PicdumpDbContext(DbContextOptions<PicdumpDbContext> opts) : base(opts)
        {

        }

        public DbSet<Picdump> Picdumps { get; set; } = null!;
    }
}
