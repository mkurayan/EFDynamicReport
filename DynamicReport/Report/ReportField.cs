using System;
using System.Text.RegularExpressions;

namespace DynamicReport.Report
{
    public class ReportField
    {
        public string Title { get; set; }

        public string SqlValueExpression { get; set; }

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

        /// <summary>
        /// Offer separates presentation from its structure.
        /// This transformation will be applied on current field before it will be passed to client.
        /// </summary>
        public virtual Func<string, string> OutputValueTransformation { get; set; }

        /// <summary>
        /// Offer separates presentation from its structure.
        /// This transformation will be applied on user input before it will be passed to server.
        /// </summary>
        public virtual Func<string, string> InputValueTransformation { get; set; }
    }
}