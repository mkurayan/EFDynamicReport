using System.Collections.Generic;
using System.Data;

namespace DynamicReport.SqlEngine
{
    public class Query
    {
        public string SqlQuery { get; set; }

        public IEnumerable<IDataParameter> Parameters { get; set; }
    }
}
