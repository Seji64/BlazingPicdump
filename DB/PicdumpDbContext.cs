using BlazingPicdump.Models;
using BlazorDB;
using Microsoft.EntityFrameworkCore;

namespace BlazingPicdump.DB
{
    public class PicdumpDbContext : BlazorDBContext
    {
        public DbSet<Picdump> Picdumps { get; set; } = null!;
    }
}
