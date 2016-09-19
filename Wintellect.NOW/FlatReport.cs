using System.Collections.Generic;
using Wintellect.Extensions;

namespace Wintellect.NOW
{
    public abstract class FlatReport<T> : Report
    {
        public FlatReport(string reportTitle, IEnumerable<T> data, 
            string subtitle = null, ReportFormat reportFormat = ReportFormat.Text, string destinationFile = null)
            : base(reportTitle, subtitle, reportFormat, destinationFile)
	    {
            data.ThrowIfNull("Argument null: data");

            Data = data;
	    }

        public IEnumerable<T> Data { get; protected set; }

        public override bool IsGroupedReport
        {
            get
            {
                return false;
            }
        }
    }
}