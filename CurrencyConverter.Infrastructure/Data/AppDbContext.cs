using CurrencyConverter.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CurrencyConverter.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<ExchangeHistory> ExchangeHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Currency>()
                .HasMany(c => c.ExchangeHistory)
                .WithOne(h => h.Currency)
                .HasForeignKey(h => h.CurId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
