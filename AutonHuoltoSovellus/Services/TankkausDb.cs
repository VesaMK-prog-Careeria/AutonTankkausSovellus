using Microsoft.EntityFrameworkCore;
using AutonHuoltoSovellus.Models;
using System.IO;
using System.Threading.Tasks;

namespace AutonHuoltoSovellus.Services
{
    public partial class TankkausDb : DbContext
    {
        public DbSet<Tankkaus> Tankkaukset { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // Tallennetaan tietokanta sovelluksen sallitulle alueelle
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "TankkausData.db");

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[TankkausDb] Käytetään tietokantaa polussa: {dbPath}");
#endif

            options.UseSqlite($"Filename={dbPath}");
        }

        public TankkausDb()
        {
            Database.EnsureCreated();
        }

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
