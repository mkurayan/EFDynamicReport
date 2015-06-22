using System.Data.Entity;
using SchoolReport.DB.Entities;

namespace SchoolReport.DB
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
        }
    }
}