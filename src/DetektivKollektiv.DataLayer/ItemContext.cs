using Microsoft.EntityFrameworkCore;

namespace DetektivKollektiv.DataLayer
{
    class ItemContext : DbContext
    {
        private const string CONNECTION_STRING = "Host=dk-database.cb7dcqyn4uwe.eu-central-1.rds.amazonaws.com;Username=postgres;Password=OhFfMhQXu0KqwtQsa0Z5;Database=postgres";

        public DbSet<Item> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CONNECTION_STRING);
    }
}
