using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolReport.DB.Entities
{
    public class School
    {
        [Key]
        public int SchoolId { get; set; }

        public string SchoolName { get; set; }

        public List<Student> Students { get; set; } 
    }
}