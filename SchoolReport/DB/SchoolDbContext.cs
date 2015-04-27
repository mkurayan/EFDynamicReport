using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SchoolReport.DB.Entities;

namespace SchoolReport.DB
{
    public class SchoolDbContext : DbContext
    {
        public DbSet<School> Schools { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Student> Examens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
              .HasRequired(x => x.School)
              .WithMany(y => y.Students)
              .WillCascadeOnDelete(true);
        }
    }
}