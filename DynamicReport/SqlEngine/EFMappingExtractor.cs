using System;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DynamicReport.SqlEngine
{
    /// <summary>
    /// Mapping Between Types & Tables
    /// </summary>
    public class EFMappingExtractor
    {
        private readonly DbContext _context;

        public EFMappingExtractor(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            _context = context;
        }

        public string GetSQLColumnName(LambdaExpression propertyLambda)
        {
            var rootObjectType = GetTheRootObjectType(propertyLambda);
            var propertyInfo = GetPropertyInfo(propertyLambda);

            foreach (ScalarPropertyMapping m in FindMappingFragment(rootObjectType).PropertyMappings)
            {
                if (propertyInfo.Name == m.Property.Name)
                {
                    return m.Column.Name;
                }
            }

            throw new ArgumentException(string.Format("Mapping not found. Entity: {0} Property {1} ", rootObjectType.Name, propertyInfo.Name));
        }

        public string GetSQLTableName(Type type)
        {
            // Find the storage entity set (table) that the entity is mapped
            var table = FindMappingFragment(type)
                .StoreEntitySet;

            // Return the table name from the storage entity set
            return (string)table.MetadataProperties["Table"].Value ?? table.Name;
        }

        private MappingFragment FindMappingFragment(Type type)
        {
            var entitySetMapping = FindTheMappingBetweenConceptualAndStorageModel(type);

            return entitySetMapping
                .EntityTypeMappings.Single()
                .Fragments.Single();
        }

        /// <summary>
        /// Find the mapping between conceptual and storage model for this entity set
        /// </summary>
        /// <param name="type"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private EntitySetMapping FindTheMappingBetweenConceptualAndStorageModel(Type type)
        {
            var metadata = ((IObjectContextAdapter)_context).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get the entity type from the model that maps to the CLR type
            var entityType = metadata
                    .GetItems<EntityType>(DataSpace.OSpace)
                    .Single(e => objectItemCollection.GetClrType(e) == type);

            // Get the entity set that uses this entity type
            var entitySet = metadata
                .GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets
                .Single(s => s.ElementType.Name == entityType.Name);

            // Find the mapping between conceptual and storage model for this entity set
            var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                    .Single()
                    .EntitySetMappings
                    .Single(s => s.EntitySet == entitySet);

            return mapping;
        }

        public static Type GetTheRootObjectType(LambdaExpression exp)
        {
            Expression body = GetMemberExpression(exp);

            // "descend" toward's the root object reference:
            while (body is MemberExpression)
            {
                var memberExpr = body as MemberExpression;
                body = memberExpr.Expression;
            }

            return body.Type;
        }

        private static PropertyInfo GetPropertyInfo(LambdaExpression propertyLambda)
        {
            MemberExpression member = GetMemberExpression(propertyLambda);
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            return propInfo;
        }

        private static MemberExpression GetMemberExpression(LambdaExpression exp)
        {
            var body = exp.Body as MemberExpression;

            if (body == null)
            {
                var ubody = (UnaryExpression)exp.Body;
                body = ubody.Operand as MemberExpression;
            }

            return body;
        }
    }
}