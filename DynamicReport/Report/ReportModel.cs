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
        public IEnumerable<IReportField> ReportFields
        {
            get { return Fields.AsEnumerable(); }
        }

        /// <summary>
        /// Set of fields available for report.
        /// </summary>
        private IList<IReportField> Fields { get; set; }

        /// <summary>
        /// Report SQL query.
        /// </summary>
        /// <returns></returns>
        private IReportDataSource DataSource { get; set; }

        private readonly IQueryBuilder _queryBuilder;

        private readonly IQueryExecutor _queryExecutor;

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

            Fields = new List<IReportField>();
        }

        public void AddReportField(IReportField reportField)
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

        public IReportField GetReportField(string title)
        {
            return Fields.FirstOrDefault(x => x.Title == title);
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
        
        public List<Dictionary<string, object>> Get(IEnumerable<IReportField> fields, IEnumerable<IReportFilter> filters)
        {
            if (fields == null)
            {
                throw new ArgumentNullException("fields");
            }

            if (filters == null)
            {
                throw new ArgumentNullException("filters");
            }

            var errors = Validate(fields, filters);
            if (errors.Length > 0)
            {
                string errorMessage = FormatReportErrors(errors);
                throw new ReportException(errorMessage);
            }

            //ToDo: check that DataSource not null!!!
            var query = _queryBuilder.BuildQuery(fields, filters, DataSource.SqlQuery);

            var data = _queryExecutor.ExecuteToDataTable(query);

            return GetJson(data);
        }

        /// <summary>
        /// Check that report can process specified fields and filters.
        /// </summary>
        /// <param name="fields">Fields which proposed for report.</param>
        /// <param name="filters">Filters which proposed for report.</param>
        /// <returns>Array with errors.</returns>
        public string[] Validate(IEnumerable<IReportField> fields, IEnumerable<IReportFilter> filters)
        {
            var errors = new List<string>();

            /****************************
             *  1. Validate ReportModel *
             ****************************/
            if (!ReportFields.Any())
            {
                errors.Add("ReportModel do not contains any ReportFields inside");
            }

            if (DataSource == null)
            {
                errors.Add("DataSource for ReportModel not specified");
            }

            /****************************
             *  2. Validate Inputs       *
             ****************************/
            
            if (!fields.Any())
            {
                errors.Add("Report must have at least one output column.");
            }

            foreach (var field in fields)
            {
                if (!Fields.Contains(field))
                {
                    errors.Add("Unknow report filed: " + field.Title);
                }
            }

            foreach (var filter in filters)
            {
                if (!Fields.Contains(filter.ReportField))
                {
                    errors.Add("Unknow report filter, field: " + filter.ReportField.Title);
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

            var reportFields = new IReportField[dt.Columns.Count];

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
    }
}