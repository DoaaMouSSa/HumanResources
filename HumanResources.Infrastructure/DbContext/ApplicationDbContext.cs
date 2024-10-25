﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

            // Configure additional entity properties if needed
            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });
            modelBuilder.Entity<Employee>()
    .Property(e => e.Id)
    .ValueGeneratedNever();

        }
        public DbSet<Department> DepartmentTbl { get; set; }
        public DbSet<Employee> EmployeeTbl { get; set; }
        public DbSet<Attendance> AttendanceTbl { get; set; }
        public DbSet<AttendanceDetails> AttendanceDetailsTbl { get; set; }
    }
}
