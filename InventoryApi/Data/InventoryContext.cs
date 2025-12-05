using Microsoft.EntityFrameworkCore;
using InventoryApi.Models;

namespace InventoryApi.Data
{
    public class InventoryContext : DbContext
    {
        public InventoryContext(DbContextOptions<InventoryContext> options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Seed sample data (optional but handy for demo)
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Periféricos" },
                new Category { Id = 2, Name = "Monitores" },
                new Category { Id = 3, Name = "Acessórios" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Mouse Óptico", Price = 49.90M, CategoryId = 1, CreatedAt = DateTime.UtcNow },
                new Product { Id = 2, Name = "Teclado Mecânico", Price = 249.90M, CategoryId = 1, CreatedAt = DateTime.UtcNow }
            );
        }
    }
}
