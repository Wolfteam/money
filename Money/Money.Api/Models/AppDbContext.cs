using Microsoft.EntityFrameworkCore;
using Money.Api.Models.Entities;
using System;

namespace Money.Api.Models
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }

        //Just for testing
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            builder.Entity<User>().HasData(
                new User
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy = "system",
                    LastName = "Bastidas",
                    Name = "Efrain",
                    Id = 1
                }, new User
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy = "system",
                    LastName = "Bastidas",
                    Name = "Elena",
                    Id = 2
                });
        }
    }
}
