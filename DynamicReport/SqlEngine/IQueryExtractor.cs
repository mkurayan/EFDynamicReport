using System;
using System.Linq.Expressions;

namespace DynamicReport.SqlEngine
{
    public interface IQueryExtractor
    {
        string GetSQLColumnName(LambdaExpression propertyLambda);

        string GetSQLTableName(Type type);
    }
}
