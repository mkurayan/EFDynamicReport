using System.ComponentModel.DataAnnotations;

namespace DynamicReport.Demo.DB.Entities
{
    public class ExamenResult
    {
        [Key]
        public int ExamenResultId { get; set; }

        public int StudentId { get; set; }

        public int SubjectId { get; set; }

        public int Score { get; set; }

        public Student Student { get; set; }

        public Subject Subject { get; set; }
    }
}