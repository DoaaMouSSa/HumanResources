using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanResources.Domain.Entities;
using System.Xml;

namespace HumanResources.Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique constraint on Employee.Code
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Code)
                .IsUnique();

            // Configure relationship between Employee.Code and Attendance.EmployeeCode
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EmployeeCode)
                .HasPrincipalKey(e => e.Code) // Reference the unique key
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes



        }
        public DbSet<Department> DepartmentTbl { get; set; }
        public DbSet<Employee> EmployeeTbl { get; set; }
        public DbSet<Week> WeekTbl { get; set; }
        public DbSet<Attendance> AttendanceTbl { get; set; }
        public DbSet<AttendanceDetails> AttendanceDetailsTbl { get; set; }
        public DbSet<Loan> LoanTbl { get; set; }
       public DbSet<Bonus> BonusTbl { get; set; }
    }
}
