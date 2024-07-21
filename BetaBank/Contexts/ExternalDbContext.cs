using BetaBank.Models;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Contexts
{
    public class ExternalDbContext : DbContext
    {
        public ExternalDbContext(DbContextOptions<ExternalDbContext> options) : base(options)
        {
        }

        public DbSet<BankCardForExternal> BankCards { get; set; } = null!;

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //}

    }
}
