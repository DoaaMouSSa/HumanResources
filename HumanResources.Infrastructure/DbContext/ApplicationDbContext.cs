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
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Employee>()
        //        .HasIndex(e => e.Code)
        //        .IsUnique();
         
        //}
        public DbSet<Department> DepartmentTbl { get; set; }
        public DbSet<Employee> EmployeeTbl { get; set; }
        public DbSet<Attendance> AttendanceTbl { get; set; }
        public DbSet<AttendancDetails> AttendanceDetailsTbl { get; set; }
    }
}
