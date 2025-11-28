using DatingWeb.Data.DbModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatingWeb.Data
{
    public class ApplicationDbContext : DbContext //IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers", "identity");
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(u => u.PhoneNumber).IsUnique();
            });
            modelBuilder.Entity<IdentityUserClaim<long>>().ToTable("AspNetUserClaims", "identity");
            modelBuilder.Entity<IdentityUserLogin<long>>().ToTable("AspNetUserLogins", "identity").HasKey(x => x.UserId);
            modelBuilder.Entity<IdentityUserToken<long>>().ToTable("AspNetUserTokens", "identity").HasKey(x => x.UserId);

            modelBuilder.Entity<IdentityRole<long>>().ToTable("AspNetRoles", "identity");
            modelBuilder.Entity<IdentityRoleClaim<long>>().ToTable("AspNetRoleClaims", "identity");
            modelBuilder.Entity<IdentityUserRole<long>>().ToTable("AspNetUserRoles", "identity").HasKey(x => x.RoleId);
            modelBuilder.Entity<Location>().ToTable("Locations", "identity").HasKey(x => x.Id);

            modelBuilder.Entity<Gallery>().ToTable("Gallery", "post");
            modelBuilder.Entity<Match>().ToTable("Match", "post");
            modelBuilder.Entity<Message>().ToTable("Message", "post");
            modelBuilder.Entity<PremiumUser>().ToTable("PremiumUser", "post");
            modelBuilder.Entity<Setting>().ToTable("Setting", "post");
            modelBuilder.Entity<Report>().ToTable("Report", "post");
            modelBuilder.Entity<Story>().ToTable("Story", "post");
            modelBuilder.Entity<Privacy>().ToTable("Privacy", "post");

            // Event System Configuration
            modelBuilder.Entity<Interest>().ToTable("Interests", "post");
            modelBuilder.Entity<Interest>().HasIndex(i => i.Name).IsUnique();

            modelBuilder.Entity<UserInterest>().ToTable("UserInterests", "post")
                .HasKey(ui => new { ui.UserId, ui.InterestId });

            modelBuilder.Entity<Event>().ToTable("Events", "post");
            modelBuilder.Entity<Event>()
                .Property(e => e.Status)
                .HasConversion<string>(); // Store enum as string

            modelBuilder.Entity<EventParticipant>().ToTable("EventParticipants", "post")
                .HasKey(ep => new { ep.EventId, ep.UserId });

            modelBuilder.Entity<CreditTransaction>().ToTable("CreditTransactions", "post");
            modelBuilder.Entity<CreditProduct>().ToTable("CreditProducts", "post");
            modelBuilder.Entity<MissedEventHistory>().ToTable("MissedEventsHistory", "post");
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Gallery> Gallery { get; set; }
        public DbSet<Match> Match { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<PremiumUser> PremiumUser { get; set; }
        public DbSet<Setting> Setting { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<Story> Story { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Privacy> Privacy { get; set; }

        // Event System DbSets
        public DbSet<Event> Events { get; set; }
        public DbSet<Interest> Interests { get; set; }
        public DbSet<UserInterest> UserInterests { get; set; }
        public DbSet<EventParticipant> EventParticipants { get; set; }
        public DbSet<CreditTransaction> CreditTransactions { get; set; }
        public DbSet<CreditProduct> CreditProducts { get; set; }
        public DbSet<MissedEventHistory> MissedEventsHistory { get; set; }
    }
}
