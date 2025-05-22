using Microsoft.EntityFrameworkCore;

namespace TeamLongestPeriod.Models
{
    public class TeamLongestPeriodDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        public DbSet<Output> Output { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("InMemoryDb");

            //base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasKey(x => x.ID);
            modelBuilder.Entity<Output>().HasKey(x => x.ID);
        }
    }
}
