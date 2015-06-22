using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolReport.DB.Entities
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string HomeAdress { get; set; }

        public string Phone { get; set; }

        public DateTime DayOfBirth { get; set; }

        public int SchoolYear { get; set; }

        public School School { get; set; }

        public List<Subject> Subjects { get; set; }  
    }
}