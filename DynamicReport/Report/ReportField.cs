using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using DynamicReport.SqlEngine;

namespace DynamicReport.Report
{
    public interface ILambdaConvertible
    {
        //LambdaExpression[] LambdaExpressions { get; }

        void ConverLambdaExpressionToSql(SqlConverter queryBuilder);
    }

    public abstract class ReportField : ILambdaConvertible
    {
        public abstract string Title { get; }
        public abstract ReportFieldType ReportFieldType { get; }
        public abstract LambdaExpression[] OuterExpressions { get; }
        public abstract string SqlTemplate { get; }

        //ToDo: rewrite all fields that use this TableAliace
        public string TableAliace;

        /// <summary>
        /// Sql alias which will be used for current field. 
        /// Example: Select (p.FirstName + p.LastName) as fullName From .....
        /// </summary>
        public string SqlAlias
        {
            get
            {
                return new Regex("[^a-zA-Z0-9]").Replace(Title, string.Empty);
            }
        }

        private string _sqlValueExpression;
        public string SqlValueExpression
        {
            get
            {
                if (_sqlValueExpression == null)
                {
                    throw new ReportException(string.Format("Sql value expression is not initialized. Field: {0}", Title));
                }

                return _sqlValueExpression;
            }

            protected set { _sqlValueExpression = value; }
        }

        /// <summary>
        /// Offer separates presentation from its structure.
        /// This transformation will be applied on current field before it will be passed to client.
        /// </summary>
        public virtual string OutputValueTransformation(string value)
        {
            return value;
        }

        /// <summary>
        /// Offer separates presentation from its structure.
        /// This transformation will be applied on user input before it will be passed to server.
        /// </summary>
        public virtual string InputParameterValueTransformation(string value)
        {
            return value;
        }

        /// <summary>
        /// Depend on user input returns sql expression for search condition.
        /// </summary>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        public virtual string GetSearchCondition(string inputContext, ReportFilterType filterType, string paramName)
        {
            return null;
        }

        public void ConverLambdaExpressionToSql(SqlConverter queryBuilder)
        {
            var sqlFields = OuterExpressions.Select(x => queryBuilder.LambdaExpressionToColumnName(x, TableAliace)).ToArray();

            SqlValueExpression = "(" + string.Format(SqlTemplate, sqlFields) + ")";
        }
    }

    public class SimpleLinqField : ReportField
    {
        private string _title;
        public override string Title
        {
            get { return _title; }
        }

        private ReportFieldType _reportFieldType;
        public override ReportFieldType ReportFieldType
        {
            get { return _reportFieldType; }
        }

        private LambdaExpression[] _outerExpressions;
        public override LambdaExpression[] OuterExpressions
        {
            get { return _outerExpressions; }
        }

        private string _sqlTemplate;
        public override string SqlTemplate
        {
            get { return _sqlTemplate; }
        }

        public SimpleLinqField(string title, string sqlTemplate, ReportFieldType reportFieldType, params LambdaExpression[] properties)
        {
            _title = title;
            _reportFieldType = reportFieldType;

            _outerExpressions = properties;
            _sqlTemplate = sqlTemplate;
        }
    }

    public abstract class ComplicatedLinqField : ReportField
    {
        SqlConverter _sqlConverter = new SqlConverter(new EFMappingExtractor(new SnapContext()));

        protected string Column<TSource, TProperty>(Expression<Func<TSource, TProperty>> exp, string tableAlias)
        {
            return _sqlConverter.LambdaExpressionToColumnName(exp, tableAlias);
        }

        protected string Column<TSource, TProperty>(Expression<Func<TSource, TProperty>> exp)
        {
            return _sqlConverter.LambdaExpressionToColumnName(exp, null);
        }

        protected string Table(Type t, string tableAlias)
        {
            return _sqlConverter.TypeToTableName(t, tableAlias);
        }

        protected string Table(Type t)
        {
            return _sqlConverter.TypeToTableName(t, null);
        }

        protected LambdaExpression Lambda<TSource, TProperty>(Expression<Func<TSource, TProperty>> exp)
        {
            return exp;
        }

        public override ReportFieldType ReportFieldType
        {
            get { return ReportFieldType.General; }
        }
    }
}