using System.Collections.Generic;
using System.Data.Entity;
using DynamicReport.Demo.DB.Entities;
using DynamicReport.Demo.Models.SchoolReportsMapping.Fields;
using DynamicReport.MappingHelpers;
using DynamicReport.Report;
using DynamicReport.SqlEngine;

namespace DynamicReport.Demo.Models.SchoolReportsMapping
{
    public class StudentsReport : ReportTemplate
    {
        private readonly IQueryExtractor queryExtractor;
        public TableMapper<Student> StudentTable;

        public StudentsReport(DbContext context)
            : base(new QueryBuilder(), new QueryExecutor(context.Database.Connection.ConnectionString))
        {
            queryExtractor = new QueryExtractor(context);
            StudentTable = new TableMapper<Student>(queryExtractor, "s");
        }

        private IEnumerable<IReportField> _reportFields;
        public override IEnumerable<IReportField> ReportFields
        {
            get
            {
                if (_reportFields == null)
                {
                    _reportFields = new List<IReportField>
                    {
                        StudentTable.Field("First Name", x => x.FirstName),
                        StudentTable.Field("Last Name", x => x.LastName),
                        StudentTable.Field("Phone", x => x.Phone),
                        StudentTable.Field("Home Adress", x => x.HomeAdress),
                        new AverageScore(queryExtractor, StudentTable).Field("Average Score"),
                        new MinimumScore(queryExtractor, StudentTable).Field("Minimum Score"),
                        new MaximumScore(queryExtractor, StudentTable).Field("Maximum Score"),
                        new AgeField(StudentTable).Field("Age"),
                        new SubjectsField(queryExtractor, StudentTable).Field("Subjects")
                    };
                }

                return _reportFields;
            }
        }

        protected override IReportDataSource ReportDataSource
        {
            get { return StudentTable.GetReportDataSource(); }
        }
    }
}