using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DynamicReport.Demo.DB.Entities
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        public string SubjectName { get; set; }

        public List<ExamenResult> ExamenResults { get; set; }  
    }
}