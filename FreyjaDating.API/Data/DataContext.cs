using FreyjaDating.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FreyjaDating.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }

        // Creates a convention using Fluent API
        // User has one Like(r) with many Like(es)
        // User has one Like(e) with many Like(re)
        // two one-to-many. And Enity framework code dont have a system to define 
        // many-to-many relationship at this time
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Like>()
                .HasKey(k => new { k.LikerId, k.LikeeId });
            
            builder.Entity<Like>()
                .HasOne(u=> u.Likee)
                .WithMany(u=> u.Liker)
                .HasForeignKey(u=> u.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                .HasOne(u=> u.Liker)
                .WithMany(u=> u.Likee)
                .HasForeignKey(u=> u.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

        }           
       

    }
}