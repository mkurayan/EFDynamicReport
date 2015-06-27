using System;
using System.Text.RegularExpressions;

namespace DynamicReport.Report
{
    public class ReportField
    {
        public string Title { get; internal set; }

        public string SqlValueExpression { get; set; }

        /// <summary>
        /// Sql alias which will be used for current field. 
        /// Example: Select (p.FirstName + p.LastName) as fullName From ...
        /// </summary>
        public string SqlAlias
        {
            get
            {
                return new Regex("[^a-zA-Z0-9]").Replace(Title, string.Empty);
            }
        }

        /// <summary>
        /// This transformation will be applied on current field value before it will be passed to client.
        /// Allow to apply any custom transformation, examples: 
        /// 1. Format data from DB format to customer format
        /// 2. Apply any calculation
        /// 3. Obfuscate or Encrypt sensitive data
        /// 4. Other usages...
        /// </summary>
        public virtual Func<string, string> OutputValueTransformation { get; set; }

        /// <summary>
        /// This transformation will be applied on user input before it will be used as filter value.
        /// Allow to apply any custom transformation, examples: 
        /// 1. Format data from customer format to DB format
        /// 2. Apply any calculation
        /// 3. Decrypt sensitive data
        /// 4. Other usages...
        /// </summary>
        public virtual Func<string, string> InputValueTransformation { get; set; }
    }
}