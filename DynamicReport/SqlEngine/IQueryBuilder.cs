﻿using System.Collections.Generic;
using System.Data.SqlClient;
using DynamicReport.Report;

namespace DynamicReport.SqlEngine
{
    public interface IQueryBuilder
    {
        SqlCommand BuildQuery(IEnumerable<IReportColumn> columns, IEnumerable<IReportFilter> filters, string dataSource);
    }
}
