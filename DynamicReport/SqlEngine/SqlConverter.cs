using System;
using System.Linq.Expressions;

namespace DynamicReport.SqlEngine
{
    public class SqlConverter
    {
        private readonly EFMappingExtractor _efMappingExtractor;
        private readonly string _prefix;

        public SqlConverter(EFMappingExtractor efMappingExtractor) : this(efMappingExtractor, Guid.NewGuid().ToString())
        {
        }

        public SqlConverter(EFMappingExtractor efMappingExtractor, string prefix)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            _efMappingExtractor = efMappingExtractor;
            _prefix = prefix;
        }

        public string LambdaExpressionToColumnName(LambdaExpression propertyLambda)
        {
            return string.Format("{0}.{1}", GetTableAlias(propertyLambda), _efMappingExtractor.GetSQLColumnName(propertyLambda));
        }

        public string TypeToTableName(Type t)
        {
            return string.Format("{0} {1}", _efMappingExtractor.GetSQLTableName(t), GetTableAlias(t));
        }

        private string GetTableAlias(LambdaExpression propertyLambda)
        {
            var t = EFMappingExtractor.GetTheRootObjectType(propertyLambda);

            return GetTableAlias(t);
        }

        private string GetTableAlias(Type t)
        {
            //Default alias.
            return _prefix + "_" + t.Name;
        }
    }
}