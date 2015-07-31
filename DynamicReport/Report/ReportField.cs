using System;
using System.Text.RegularExpressions;

namespace DynamicReport.Report
{
    public class ReportField : IReportField
    {
        public string Title { get; set; }

        public string SqlValueExpression { get; set; }

        public string SqlAlias
        {
            get
            {
                return new Regex("[^a-zA-Z0-9]").Replace(Title, string.Empty);
            }
        }

        public Func<string, string> OutputValueTransformation { get; set; }

        public Func<string, string> InputValueTransformation { get; set; }

        /// <summary>
        /// Create empty report field. 
        /// </summary>
        public ReportField()
        {
            //set default transformations.
            OutputValueTransformation = x => x;
            InputValueTransformation = x => x;
        }

        public bool Equals(ReportField other)
        {
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            //check only Title & SqlValueExpression because InputValueTransformation & OutputValueTransformation can not affect query structure.
            return Title == other.Title
                && SqlValueExpression == other.SqlValueExpression;
        }

        public override bool Equals(object other)
        {    
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return this.Equals(other as ReportField);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Title.GetHashCode();
            hash = (hash * 7) + SqlValueExpression.GetHashCode();

            return hash;
        }

        public static bool operator ==(ReportField lhs, ReportField rhs)
        {
            // Check for null on left side. 
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true. 
                    return true;
                }

                // Only the left side is null. 
                return false;
            }
            // Equals handles case of null on right side. 
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ReportField lhs, ReportField rhs)
        {
            return !(lhs == rhs);
        }
    }
}