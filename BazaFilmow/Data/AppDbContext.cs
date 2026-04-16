using Microsoft.EntityFrameworkCore;
using BazaFilmow.Models;

namespace BazaFilmow.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Film> Filmy { get; set; }
    }
}
