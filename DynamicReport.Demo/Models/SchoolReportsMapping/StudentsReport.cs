using System.Collections.Generic;
using System.Data.Entity;
using DynamicReport.Demo.DB.Entities;
using DynamicReport.Demo.Models.SchoolReportsMapping.Columns;
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

        private IEnumerable<IReportColumn> _reportColumns;
        public override IEnumerable<IReportColumn> ReportColumns
        {
            get
            {
                if (_reportColumns == null)
                {
                    _reportColumns = new List<IReportColumn>
                    {
                        StudentTable.Column("First Name", x => x.FirstName),
                        StudentTable.Column("Last Name", x => x.LastName),
                        StudentTable.Column("Phone", x => x.Phone),
                        StudentTable.Column("Home Adress", x => x.HomeAdress),
                        new AverageScore(queryExtractor, StudentTable).Column("Average Score"),
                        new MinimumScore(queryExtractor, StudentTable).Column("Minimum Score"),
                        new MaximumScore(queryExtractor, StudentTable).Column("Maximum Score"),
                        new Age(StudentTable).Column("Age"),
                        new Subjects(queryExtractor, StudentTable).Column("Subjects")
                    };
                }

                return _reportColumns;
            }
        }

        protected override IReportDataSource ReportDataSource
        {
            get { return StudentTable.GetReportDataSource(); }
        }
    }
}