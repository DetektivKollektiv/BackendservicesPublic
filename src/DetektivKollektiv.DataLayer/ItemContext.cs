using Microsoft.EntityFrameworkCore;

namespace DetektivKollektiv.DataLayer
{
    class ItemContext : DbContext
    {
        public DbSet<Item> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=dk-database.cb7dcqyn4uwe.eu-central-1.rds.amazonaws.com;Username=postgres;" +
                "Password=OhFfMhQXu0KqwtQsa0Z5;Database=postgres");
    }
}
