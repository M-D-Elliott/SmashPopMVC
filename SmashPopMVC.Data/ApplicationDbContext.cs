using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmashPopMVC.Data.Models;

namespace SmashPopMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Tally> Tallies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.RequestedBy)
                .WithMany(u => u.SentFriendRequests)
                .HasForeignKey(f => f.RequestedById);

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.RequestedTo)
                .WithMany(u => u.ReceievedFriendRequests);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ReplyTo)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ReplyToID);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Postee)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.PosteeID);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.MostDifficult)
                .WithMany(ch => ch.MostDifficultVotes)
                .HasForeignKey(v => v.MostDifficultID);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.LeastDifficult)
                .WithMany(ch => ch.LeastDifficultVotes)
                .HasForeignKey(v => v.LeastDifficultID);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.FlavorOfTheMonth)
                .WithMany(ch => ch.FlavorOfTheMonthVotes)
                .HasForeignKey(v => v.FlavorOfTheMonthID);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.MostPowerful)
                .WithMany(ch => ch.MostPowerfulVotes)
                .HasForeignKey(v => v.MostPowerfulID);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Voter)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.VoterID);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Tally)
                .WithMany(t => t.Votes)
                .HasForeignKey(v => v.TallyID);
        }
    }
}
