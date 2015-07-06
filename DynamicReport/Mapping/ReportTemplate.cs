using System.Collections.Generic;
using System.Data.Entity;
using DynamicReport.Report;
using DynamicReport.SqlEngine;

namespace DynamicReport.Mapping
{
    //ToDo: Implement
    public abstract class ReportTemplate
    {
        public abstract IEnumerable<ReportField> ReportFields { get; }

        /// <summary>
        /// Data source for report.
        /// </summary>
        protected abstract ReportDataSource ReportDataSource { get; }

        protected DbContext _context;
        protected ReportTemplate(DbContext context)
        {
            _context = context;
        }

        public ReportModel CreateReport()
        {
            var model = new ReportModel(
              new QueryBuilder(),
              new QueryExecutor(_context.Database.Connection.ConnectionString));

            model.SetDataSource(ReportDataSource);

            foreach (var repotField in ReportFields)
            {
                model.AddReportField(repotField);
            }

            return model;
        }
    }
}
