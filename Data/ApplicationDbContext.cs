using CarWashOps.Models;
using Microsoft.EntityFrameworkCore;

namespace CarWashOps.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Wash> Washes => Set<Wash>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Client> Clients => Set<Client>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Wash>().ToTable("Washes");
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Client>().ToTable("Clients");

            // Wash -> Employee (many washes per employee, optional so deleting
            // an employee never deletes their wash history)
            modelBuilder.Entity<Wash>()
                .HasOne(w => w.Employee)
                .WithMany(e => e.Washes)
                .HasForeignKey(w => w.EmployeeNum)
                .OnDelete(DeleteBehavior.SetNull);

            // Wash -> Client (many washes per client, optional for walk-ins)
            modelBuilder.Entity<Wash>()
                .HasOne(w => w.Client)
                .WithMany(c => c.Washes)
                .HasForeignKey(w => w.ClientId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Employee>().Property(e => e.Status).HasDefaultValue("Active");
            modelBuilder.Entity<Wash>().Property(w => w.Status).HasDefaultValue("In Progress");
            modelBuilder.Entity<Wash>().Property(w => w.WashDate).HasDefaultValueSql("GETDATE()");
        }
    }
}
