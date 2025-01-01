namespace TaskListApp.Models
{
    using Microsoft.EntityFrameworkCore;
    using TaskListApp.Models;

    public class TaskDbContext : DbContext
    {
        public DbSet<Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
        
        /*To accept DbContextOptions*/
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TaskManager;Trusted_Connection=True;");
        }

        /*For the foreign keys*/
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>()
                .HasOne(t => t.Assignee)
                .WithMany()
                .HasForeignKey(t => t.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict); //No cascading delete for Assignee

            modelBuilder.Entity<Task>()
                .HasOne(t => t.Reviewer)
                .WithMany()
                .HasForeignKey(t => t.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict); //No cascading delete for Reviewer
        }

    }
}
