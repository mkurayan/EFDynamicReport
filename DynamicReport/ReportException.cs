using System;

namespace DynamicReport
{
    public class ReportException : Exception
    {
        public ReportException(string message)
            : base(message)
        {
        }
    }
}