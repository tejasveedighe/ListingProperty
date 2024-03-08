using ListingProperty.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;

namespace ListingProperty.Data
{
    public class AppContextDb : DbContext
    {
        public AppContextDb(DbContextOptions<AppContextDb> options) : base(options)
        {
        }


        public DbSet<User> LpUser { get; set; }
        public DbSet<Property> lpProperty { get; set; }
        public DbSet<FavoriteProperty> LpFavProperty { get; set; }
        public DbSet<Image> LpImages { get; set; }
        public DbSet<ContactApproval> LpContactApproval { get; set; }
        public DbSet<OfferModal> LpPropertyOffers { get; set; }
        public DbSet<Payments> LpPayments { get; set; }
        public DbSet<OwnedProperties> LpOwnedProperties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .HasOne(f => f.FavoriteProperty)
            .WithOne(p => p.Users)
            .HasForeignKey<FavoriteProperty>(f => f.UserId);

            modelBuilder.Entity<Property>()
          .HasOne(f => f.FavoriteProperty)
          .WithOne(p => p.Property)
          .HasForeignKey<FavoriteProperty>(f => f.PropertyId);
        }
    }
}
