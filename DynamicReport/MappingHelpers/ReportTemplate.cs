using System.Collections.Generic;
using DynamicReport.Report;
using DynamicReport.SqlEngine;

namespace DynamicReport.MappingHelpers
{
    public abstract class ReportTemplate
    {
        public abstract IEnumerable<IReportColumn> ReportColumns { get; }

        /// <summary>
        /// Data source for report.
        /// </summary>
        protected abstract IReportDataSource ReportDataSource { get; }


        private readonly IQueryExecutor _queryExecutor;
        private readonly IQueryBuilder _queryBuilder;

        protected ReportTemplate(IQueryBuilder queryBuilder, IQueryExecutor queryExecutor)
        {
            _queryExecutor = queryExecutor;
            _queryBuilder = queryBuilder;
        }

        public IReportModel CreateReport()
        {
            var model = new ReportModel(_queryBuilder, _queryExecutor);

            model.SetDataSource(ReportDataSource);

            foreach (var repotColumn in ReportColumns)
            {
                model.AddReportColumn(repotColumn);
            }

            return model;
        }
    }
}
