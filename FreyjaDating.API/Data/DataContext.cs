using FreyjaDating.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FreyjaDating.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) 
            : base(options) {}

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        // Creates a convention using Fluent API
        // User has one Like(r) with many Like(es)
        // User has one Like(e) with many Like(re)
        // two one-to-many. And Enity framework code dont have a system to define 
        // many-to-many relationship at this time
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasMany(p => p.Photos)
                .WithOne(u => u.User)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Like>()
                .HasKey(k => new {k.LikerId, k.LikeeId});
            
            builder.Entity<Like>()
                .HasOne(u => u.Likee)
                .WithMany(u => u.Liker)
                .HasForeignKey(u => u.LikerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Like>()
                .HasOne(u => u.Liker)
                .WithMany(u => u.Likee)
                .HasForeignKey(u => u.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // many-to-many relationship entity framework
            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessageSent)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}