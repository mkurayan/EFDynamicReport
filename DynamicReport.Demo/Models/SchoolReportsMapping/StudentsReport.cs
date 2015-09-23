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
        private readonly IQueryExtractor _queryExtractor;
        private readonly TableMapper<Student> _studentTable;

        public StudentsReport(DbContext context)
            : base(new QueryBuilder(), new QueryExecutor(context.Database.Connection.ConnectionString))
        {
            _queryExtractor = new QueryExtractor(context);
            _studentTable = new TableMapper<Student>(_queryExtractor, "s");
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
                        _studentTable.Column("First Name", x => x.FirstName),
                        _studentTable.Column("Last Name", x => x.LastName),
                        _studentTable.Column("Phone", x => x.Phone),
                        _studentTable.Column("Home Adress", x => x.HomeAdress),
                        new AverageScore(_queryExtractor, _studentTable).Column("Average Score"),
                        new MinimumScore(_queryExtractor, _studentTable).Column("Minimum Score"),
                        new MaximumScore(_queryExtractor, _studentTable).Column("Maximum Score"),
                        new Age(_studentTable).Column("Age"),
                        new Subjects(_queryExtractor, _studentTable).Column("Subjects")
                    };
                }

                return _reportColumns;
            }
        }

        protected override IReportDataSource ReportDataSource
        {
            get { return _studentTable.GetReportDataSource(); }
        }
    }
}