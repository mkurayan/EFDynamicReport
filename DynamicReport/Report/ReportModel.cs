using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DynamicReport.SqlEngine;

namespace DynamicReport.Report
{
    public class ReportModel : IReportModel
    {
        /// <summary>
        /// Set of columns available for report.
        /// </summary>
        public IEnumerable<IReportColumn> ReportColumns
        {
            get { return _reportColumns.AsEnumerable(); }
        }

        /// <summary>
        /// Report SQL query.
        /// </summary>
        /// <returns></returns>
        private IReportDataSource DataSource { get; set; }

        private readonly IQueryBuilder _queryBuilder;

        private readonly IQueryExecutor _queryExecutor;

        private IList<IReportColumn> _reportColumns;

        public ReportModel(IQueryBuilder queryBuilder, IQueryExecutor queryExecutor)
        {
            if (queryBuilder == null)
            {
                throw new ArgumentNullException("queryBuilder");
            }

            if (queryExecutor == null)
            {
                throw new ArgumentNullException("queryExecutor");
            }

            _queryBuilder = queryBuilder;
            _queryExecutor = queryExecutor;

            _reportColumns = new List<IReportColumn>();
        }

        public void AddReportColumn(IReportColumn reportColumn)
        {
            if (reportColumn == null)
            {
                throw new ArgumentNullException("reportColumn");
            }

            if (_reportColumns.Any(x => x.SqlAlias == reportColumn.SqlAlias))
            {
                throw new ReportException(string.Format("Report model can not contain columns with equal SqlAliaces: {0}", reportColumn.SqlAlias));
            }

            _reportColumns.Add(reportColumn);
        }

        public IReportColumn GetReportColumn(string title)
        {
            return _reportColumns.FirstOrDefault(x => x.Title == title);
        }

        public void SetDataSource(IReportDataSource dataSource)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource", "Report data source can not be null");
            }

            if (string.IsNullOrEmpty(dataSource.SqlQuery))
            {
                throw new ReportException("Report data source contain empty sql query");
            }

            DataSource = dataSource;
        }
        
        public List<Dictionary<string, object>> Get(IEnumerable<IReportColumn> columns, IEnumerable<IReportFilter> filters)
        {
            if (columns == null)
            {
                throw new ArgumentNullException("columns");
            }

            if (filters == null)
            {
                throw new ArgumentNullException("filters");
            }

            var errors = Validate(columns, filters);
            if (errors.Length > 0)
            {
                string errorMessage = FormatReportErrors(errors);
                throw new ReportException(errorMessage);
            }

            //ToDo: check that DataSource not null!!!
            var query = _queryBuilder.BuildQuery(columns, filters, DataSource.SqlQuery);

            var data = _queryExecutor.ExecuteToDataTable(query);

            return GetJson(data);
        }

        /// <summary>
        /// Check that report can process specified coluns and filters.
        /// </summary>
        /// <param name="columns">Columns which proposed for report.</param>
        /// <param name="filters">Filters which proposed for report.</param>
        /// <returns>Array with errors.</returns>
        public string[] Validate(IEnumerable<IReportColumn> columns, IEnumerable<IReportFilter> filters)
        {
            var errors = new List<string>();

            /****************************
             *  1. Validate ReportModel *
             ****************************/
            if (!ReportColumns.Any())
            {
                errors.Add("ReportModel do not contains any ReportColumns inside");
            }

            if (DataSource == null)
            {
                errors.Add("DataSource for ReportModel not specified");
            }

            /****************************
             *  2. Validate Inputs       *
             ****************************/
            
            if (!columns.Any())
            {
                errors.Add("Report must have at least one output column.");
            }

            foreach (var column in columns)
            {
                if (!ReportColumns.Contains(column))
                {
                    errors.Add("Unknow report column: " + column.Title);
                }
            }

            foreach (var filter in filters)
            {
                if (!ReportColumns.Contains(filter.ReportColumn))
                {
                    errors.Add("Unknow report filter, column: " + filter.ReportColumn.Title);
                }
            }

            return errors.ToArray();
        }

        private string FormatReportErrors(string[] errors)
        {
            if (errors.Length == 0)
                return string.Empty;

            StringBuilder error = new StringBuilder();

            error.AppendLine(string.Format("Сan not build a report for the specified data. Errors count: {0}", errors.Length));
            for (var i = 0; i < errors.Length; i++)
            {
                error.AppendLine(string.Format("{0}: {1}", i, errors[i]));
            }

            return error.ToString();
        }

        private List<Dictionary<string, object>> GetJson(DataTable dt)
        {
            var rows = new List<Dictionary<string, object>>();

            var reportColumns = new IReportColumn[dt.Columns.Count];

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                reportColumns[i] = _reportColumns.Single(x => x.SqlAlias == dt.Columns[i].ColumnName);
            }

            foreach (DataRow dr in dt.Rows)
            {
                var row = new Dictionary<string, object>();

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string value = "";
                    if (dr[j] != null)
                    {
                        value = reportColumns[j].OutputValueTransformation(dr[j].ToString());
                    }

                    row.Add(dt.Columns[j].ColumnName, value);
                }

                rows.Add(row);
            }

            return rows;
        }
    }
}