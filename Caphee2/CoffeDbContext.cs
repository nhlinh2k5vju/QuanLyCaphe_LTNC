using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection.Emit;
using System.Text;

namespace DAL
{
    public class CoffeDbContext : DbContext
    {
        public CoffeDbContext(DbContextOptions<CoffeDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Promotion> Promotions => Set<Promotion>();
        public DbSet<MenuItem> MenuItems => Set<MenuItem>();
        public DbSet<MenuItemSize> MenuItemSizes => Set<MenuItemSize>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Promotion>()
                .HasIndex(p => p.Code)
                .IsUnique();
        }
    }
}
