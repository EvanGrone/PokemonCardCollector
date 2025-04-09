using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PokemonCardCollector.Models;

namespace PokemonCardCollector.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<PokemonCardCollector.Models.PokemonCard> PokemonCard { get; set; } = default!;
        public DbSet<PokemonCardCollector.Models.Collection> Collection { get; set; } = default!;
    }
}
