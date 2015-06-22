using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolReport.DB.Entities
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        public string SubjectName { get; set; }

        public List<Student> Students { get; set; }  
    }
}