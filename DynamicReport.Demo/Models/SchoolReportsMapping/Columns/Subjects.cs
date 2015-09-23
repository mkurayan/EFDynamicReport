using DynamicReport.Demo.DB.Entities;
using DynamicReport.MappingHelpers;
using DynamicReport.Report;
using DynamicReport.SqlEngine;

namespace DynamicReport.Demo.Models.SchoolReportsMapping.Columns
{
    public class Subjects
    {
        //Inner tables
        private readonly TableMapper<Subject> sb;
        private readonly TableMapper<ExamenResult> e; 

        //Outer tables
        private readonly TableMapper<Student> S;

        public Subjects(IQueryExtractor queryExtractor, TableMapper<Student> studentTable)
        {
            sb = new TableMapper<Subject>(queryExtractor, "subj");
            e = new TableMapper<ExamenResult>(queryExtractor, "subj");
            S = studentTable;
        }

        public string SqlValueExpression
        {
            get
            {
                return "substring(  " +
                       "(SELECT ', ' + " + sb.Column(s => s.SubjectName) +
                       " FROM " + sb.Table() +
                       " INNER JOIN " + e.Table() + " ON " + e.Column(x => x.SubjectId) + " = " + sb.Column(x => x.SubjectId) +
                       " WHERE " + e.Column(x => x.StudentId) + " = " + S.Column(x => x.StudentId) +
                       " FOR XML PATH ('') ), 2, 1000)";
            }
        }

        public IReportColumn Column(string title)
        {
            return new ReportColumn
            {
                Title = title,
                SqlValueExpression = SqlValueExpression
            };
        }
    }
}