using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicReport.SqlEngine
{
    public class Query
    {
        public string SqlQuery { get; set; }

        public IEnumerable<IDataParameter> Parameters { get; set; }

        public IEnumerable<string> Columns { get; set; }
    }
}
