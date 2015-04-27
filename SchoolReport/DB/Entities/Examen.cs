using System.ComponentModel.DataAnnotations;

namespace SchoolReport.DB.Entities
{
    public class Examen
    {
        [Key]
        public int ExamenId { get; set; }

        public string ExamenName { get; set; }
    }
}