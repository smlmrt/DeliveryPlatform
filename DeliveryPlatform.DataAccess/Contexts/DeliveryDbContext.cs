using DeliveryPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeliveryPlatform.DataAccess.Contexts
{
    public class DeliveryDbContext : DbContext
    {
        public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options) : base(options){}

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Courier> Couriers { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Bir restoran silindiğinde ona ait siparişler SİLİNMESİN (Restrict)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Restaurant)
                .WithMany(r => r.Orders)
                .HasForeignKey(o => o.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // 2. Bir kurye sistemden çıkarıldığında, eski siparişlerindeki KuryeId alanı NULL olsun
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Courier)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CourierId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}