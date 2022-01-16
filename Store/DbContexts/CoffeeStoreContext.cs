using CoffeeStore.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeStore.Data
{
    public class CoffeeStoreContext : DbContext
    {
        public DbSet<Coffee> Coffees { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderCoffee> OrderCoffees { get; set; }

        public CoffeeStoreContext(DbContextOptions<CoffeeStoreContext> options) :base(options) 
        {
            if (Coffees == null || Orders == null || OrderCoffees == null) 
            {
                throw new ArgumentNullException();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderCoffee>()
                .HasIndex(oc => oc.CoffeeID).IsUnique(true);
            modelBuilder.Entity<OrderCoffee>()
                .HasKey(oc => new { oc.OrderID, oc.CoffeeID });
           
            modelBuilder.Entity<OrderCoffee>()
                .HasOne(oc => oc.Order)
                .WithMany(c => c.OrderCoffees)
                .HasForeignKey(oc => oc.OrderID);
            modelBuilder.Entity<OrderCoffee>()
                .HasOne(oc => oc.Coffee)
                .WithMany(c => c.OrderCoffees)
                .HasForeignKey(oc => oc.CoffeeID);
        }
    }
}
