using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DynamicReport.SqlEngine;

namespace DynamicReport.Report
{
    public class ReportModel
    {
        public IEnumerable<ReportField> ReportFields
        {
            get { return Fields.AsEnumerable(); }
        }

        /// <summary>
        /// Set of fields available for report.
        /// </summary>
        private IList<ReportField> Fields { get; set; }

        /// <summary>
        /// Report SQL query.
        /// </summary>
        /// <returns></returns>
        private ReportDataSource DataSource { get; set; }
        
        private readonly QueryBuilder _queryBuilder;

        private readonly QueryExecutor _queryExecutor;

        public ReportModel(QueryBuilder queryBuilder, QueryExecutor queryExecutor)
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

            Fields = new List<ReportField>();
        }

        public void AddReportField(ReportField reportField)
        {
            if (reportField == null)
            {
                throw new ArgumentNullException("reportField");
            }

            if (Fields.Any(x => x.SqlAlias == reportField.SqlAlias))
            {
                throw new ReportException(string.Format("Report model can not contain fields with equal SqlAliaces: {0}", reportField.SqlAlias));
            }

            Fields.Add(reportField);
        }

        public void SetDataSource(ReportDataSource dataSource)
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
        
        /// <summary>
        /// Process report with proposed fields and filters, return report data.
        /// </summary>
        /// <param name="columns">Fields which will be included in report.</param>
        /// <param name="filters">Filters which will be applied on report.</param>
        /// <param name="hospitalId">Id of hospital for which report will be build.</param>
        public List<Dictionary<string, object>> Get(IEnumerable<string> columns, IEnumerable<ReportFilter> filters)
        {
            var error = Validate(columns, filters);
            if (!string.IsNullOrEmpty(error))
            {
                throw new ReportException(error);
            }

            var query = _queryBuilder.BuildQuery(columns.Select(x => Fields.Single(y => y.Title == x)), filters, DataSource.SqlQuery);

            var data = _queryExecutor.ExecuteToDataTable(query);

            return GetJson(data);
        }

        private List<Dictionary<string, object>> GetJson(DataTable dt)
        {
            var rows = new List<Dictionary<string, object>>();

            var reportFields = new ReportField[dt.Columns.Count];

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                reportFields[i] = Fields.Single(x => x.SqlAlias == dt.Columns[i].ColumnName);
            }

            foreach (DataRow dr in dt.Rows)
            {
                var row = new Dictionary<string, object>();

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string value = "";
                    if (dr[j] != null)
                    {
                        value = reportFields[j].OutputValueTransformation(dr[j].ToString());
                    }

                    row.Add(dt.Columns[j].ColumnName, value);
                }

                rows.Add(row);
            }

            return rows;
        }

        /// <summary>
        /// Check that report can process specified fields and filters.
        /// </summary>
        /// <param name="columnsTitles">Fields which proposed for report.</param>
        /// <param name="filters">Filters which proposed for report.</param>
        /// <returns>Validation result, null if there is no validation errors.</returns>
        private string Validate(IEnumerable<string> columnsTitles, IEnumerable<ReportFilter> filters)
        {
            var errors = new List<string>();

            if (!columnsTitles.Any())
            {
                errors.Add("Report must have at least one output column.");
            }

            foreach (var field in columnsTitles)
            {
                if (Fields.All(x => x.Title != field))
                {
                    errors.Add("Unknow report filed: " + field);
                }
            }

            foreach (var filter in filters)
            {
                if (Fields.All(x => x.Title != filter.ReportFieldTitle))
                {
                    errors.Add("Unknow report filter, field: " + filter.ReportFieldTitle);
                }
            }

            if (!errors.Any()) return null;

            StringBuilder error = new StringBuilder();
            error.AppendLine("Value of a fields or filters column is outside the allowable range of values.");
            foreach (var err in errors)
            {
                error.AppendLine(err);
            }

            return error.ToString();
        }
    }
}