using BetaBank.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Contexts
{
    public class BetaBankDbContext : IdentityDbContext<AppUser>
    {
        public BetaBankDbContext(DbContextOptions<BetaBankDbContext> options) : base(options)
        {

        }
        
        public DbSet<BankAccount> BankAccounts { get; set; } = null!;
        public DbSet<BankCard> BankCards { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<Models.News> News { get; set; } = null!;
        public DbSet<Support> Supports { get; set; } = null!;
        public DbSet<SendedNotificationMail> SendedNotificationMails { get; set; } = null!;



        public DbSet<BankCardStatus> BankCardStatuses { get; set; } = null!;
        public DbSet<BankCardType> BankCardTypes { get; set; } = null!;
        public DbSet<TransactionStatus> TransactionStatuses { get; set; } = null!;
        public DbSet<TransactionType> TransactionCardTypes { get; set; } = null!;
        public DbSet<BankAccountStatus> BankAccountStatuses { get; set; } = null!;
        public DbSet<SupportStatus> SupportStatuses { get; set; } = null!;



        public DbSet<BankCardStatusModel> BankCardStatusModels { get; set; } = null!;
        public DbSet<BankCardTypeModel> BankCardTypeModels { get; set; } = null!;
        public DbSet<TransactionStatusModel> TransactionStatusModels { get; set; } = null!;
        public DbSet<TransactionTypeModel> TransactionTypeModels { get; set; } = null!;
        public DbSet<BankAccountStatusModel> BankAccountStatusModels { get; set; } = null!;
        public DbSet<SupportStatusModel> SupportStatusModels { get; set; } = null!;
        public DbSet<Subscriber> Subscribers { get; set; } = null!;





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.SourceCard)
                .WithMany() 
                .HasForeignKey(t => t.SourceCardId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.DestinationCard)
                .WithMany() 
                .HasForeignKey(t => t.DestinationCardId)
                .OnDelete(DeleteBehavior.Restrict); 
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasIndex(e => e.FIN).IsUnique();
            });
            modelBuilder.Entity<Subscriber>(entity =>
            {
                entity.HasIndex(e => e.Mail).IsUnique();
            });

            base.OnModelCreating(modelBuilder);

            




        }

    }
}
