using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Projects.Api.Entities;

namespace Projects.Api.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) 
            : base(options)
        {
        }

        public DbSet<ProjectEntity> Projects { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProjectEntity>(pro =>
            {
                pro.HasMany(p => p.Tasks).WithOne(t => t.Project).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TaskEntity>(Tas =>
            {
                Tas.HasOne(d => d.Project).WithMany(c => c.Tasks).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
