using Microsoft.EntityFrameworkCore;
using AutonHuoltoSovellus.Models;

namespace AutonHuoltoSovellus.Services
{
    public class TankkausDb : DbContext
    {
        public DbSet<Tankkaus> Tankkaukset { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=tankkaus.db");

        public async Task<List<Tankkaus>> GetTankkauksetAsync()
        {
            return await Tankkaukset.OrderBy(t => t.Aika).ToListAsync();
        }

        public async Task PoistaTankkausAsync(Tankkaus tankkaus)
        {
            Tankkaukset.Remove(tankkaus);
            await SaveChangesAsync();
        }

        public async Task<Tankkaus?> GetViimeisinTankkausAsync()
        {
            return await Tankkaukset
                .OrderByDescending(t => t.Aika)
                .FirstOrDefaultAsync();
        }


    }
}
