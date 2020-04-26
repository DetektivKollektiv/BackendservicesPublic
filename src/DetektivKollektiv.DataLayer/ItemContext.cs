using Microsoft.EntityFrameworkCore;

namespace DetektivKollektiv.DataLayer
{
    class ItemContext : DbContext
    {
        private const string CONNECTION_STRING = "##################################################################";

        public DbSet<Item> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CONNECTION_STRING);
    }
}
