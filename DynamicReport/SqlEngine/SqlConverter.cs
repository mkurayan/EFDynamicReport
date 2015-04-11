using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DynamicReport.SqlEngine
{
    public class SqlConverter
    {
        private readonly EFMappingExtractor _efMappingExtractor;
        private readonly Dictionary<string, string>_tablesAliases;

        public SqlConverter(EFMappingExtractor efMappingExtractor)
            : this(efMappingExtractor, new Dictionary<string,Type>())
        { 
        }

        public SqlConverter(EFMappingExtractor efMappingExtractor, Dictionary<string, Type> tablesAliases)
        {
            _efMappingExtractor = efMappingExtractor;

            _tablesAliases = new Dictionary<string, string>();

            foreach (KeyValuePair<string, Type> tablesAliase in tablesAliases)
            {
                _tablesAliases.Add(tablesAliase.Key, tablesAliase.Value.Name); 
            }
        }

        public string LambdaExpressionToColumnName(LambdaExpression propertyLambda, string tableAlias = null)
        {
            return string.Format("{0}.{1}",
                tableAlias ?? GetTableAlias(propertyLambda),
                _efMappingExtractor.GetSQLColumnName(propertyLambda));
        }

        public string TypeToTableName(Type t, string tableAlias)
        {
            return string.Format("{0} {1}", _efMappingExtractor.GetSQLTableName(t),
                 tableAlias ?? GetTableAlias(t));
        }

        private string GetTableAlias(LambdaExpression propertyLambda)
        {
            var t = EFMappingExtractor.GetTheRootObjectType(propertyLambda);

            return GetTableAlias(t);
        }

        private string GetTableAlias(Type t)
        {
            if (_tablesAliases.ContainsValue(t.Name))
            {
                if (_tablesAliases.Count(x => x.Value == t.Name) > 1)
                {
                    throw new ReportException("Resolve alias error. More than one aliases suited for type. Please provide alias directly.");
                }

                return _tablesAliases.Where(x => x.Value == t.Name).Select(x => x.Key).Single();
            }

            //Default alias.
            return "_" + t.Name;
        }
    }
}