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

            modelBuilder.Entity<Gallery>().ToTable("Gallery", "post");
            modelBuilder.Entity<Match>().ToTable("Match", "post");
            modelBuilder.Entity<Message>().ToTable("Message", "post");
            modelBuilder.Entity<PremiumUser>().ToTable("PremiumUser", "post");
            modelBuilder.Entity<Setting>().ToTable("Setting", "post");
            modelBuilder.Entity<Report>().ToTable("Report", "post");
            modelBuilder.Entity<Story>().ToTable("Story","post");
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Gallery> Gallery { get; set; }
        public DbSet<Match> Match { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<PremiumUser> PremiumUser { get; set; }
        public DbSet<Setting> Setting { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<Story> Story { get; set; }
    }
}
