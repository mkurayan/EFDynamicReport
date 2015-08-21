using System.Data.Entity;
using DynamicReport.Demo.DB.Entities;

namespace DynamicReport.Demo.DB
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext()
            : base("name=SchoolDBConnectionString") 
        {
        }

        public DbSet<School> Schools { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Subject> Subjects { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
              .HasRequired(x => x.School)
              .WithMany(y => y.Students)
              .WillCascadeOnDelete(true);

            modelBuilder.Entity<Student>()
                .HasMany(x => x.ExamenResults)
                .WithRequired(x => x.Student);

            modelBuilder.Entity<Subject>()
                .HasMany(x => x.ExamenResults)
                .WithRequired(x => x.Subject);
        }
    }
}