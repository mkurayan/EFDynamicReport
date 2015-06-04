using System;
using System.Collections.Generic;
using System.Data.Entity;
using SchoolReport.DB.Entities;

namespace SchoolReport.DB
{
    public class SchoolDbInitializer : DropCreateDatabaseAlways<SchoolDbContext>
    {
        protected override void Seed(SchoolDbContext context)
        {
            var firstSchool = new School()
            {
                SchoolName = "First School"
            };

            context.SaveChanges();

            context.Schools.Add(firstSchool);

            List<Student> students = new List<Student>()
            {
                new Student()
                {
                    School = firstSchool,
                    DayOfBirth = new DateTime(1999, 2, 13),
                    FirstName = "Lara",
                    LastName = "Steem",
                    HomeAdress = "QR. St Person st. House 9.",
                    Phone = "+1 789 567 4237",
                    SchoolYear = 3,
                },

                new Student()
                {
                    School = firstSchool,
                    DayOfBirth = new DateTime(1999, 2, 13),
                    FirstName = "Jini",
                    LastName = "Steem",
                    HomeAdress = "QR. St Person st. House 9.",
                    Phone = "+1 789 167 6276",
                    SchoolYear = 3,
                },

                new Student()
                {
                    School = firstSchool,
                    DayOfBirth = new DateTime(1997, 2, 13),
                    FirstName = "Mark",
                    LastName = "Green",
                    HomeAdress = "QR. Malony st. House 11.",
                    Phone = "+1 790 542 4132",
                    SchoolYear = 4,
                },

                new Student()
                {
                    School = firstSchool,
                    DayOfBirth = new DateTime(2001, 2, 13),
                    FirstName = "Jhon",
                    LastName = "Crow",
                    HomeAdress = "QR. Malony st. House 28.",
                    Phone = "+1 790 647 8473",
                    SchoolYear = 1,
                },
            };

            context.Students.AddRange(students);
        }
    }
}