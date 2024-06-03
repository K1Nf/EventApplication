using EventApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace EventApplication.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Role> Roles { get; set; }
        //public DbSet<UserType> UserTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasMany(u=>u.Tags).WithMany(t => t.Users).UsingEntity("UsersTags");

            modelBuilder.Entity<Event>().HasMany(ev=>ev.Tags).WithMany(t => t.Events).UsingEntity("EventsTags");


            
            base.OnModelCreating(modelBuilder);
        }
    }
}
