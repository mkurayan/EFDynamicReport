namespace SchoolReport.Models
{
    public class ReportTypeDTO
    {
        public string Type { get; set; }
    }

    public class ReportFilterDTO
    {
        public string FilterTitle { get; set; }

        public int FilterType { get; set; }
    }

    public class ReportColumnDTO
    {
        public string Title { get; set; }

        public string Alias { get; set; }
    }

    public class FilterDTO
    {
        public string ReportFieldTitle { get; set; }
        public int FilterType { get; set; }
        public string FilterValue { get; set; }
    }

    public class ReportDTO
    {
        public string[] Columns { get; set; }

        public FilterDTO[] Filters  { get; set; }
    }

}