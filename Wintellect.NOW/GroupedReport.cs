using System.Collections.Generic;
using System.Linq;
using Wintellect.Extensions;

namespace Wintellect.NOW
{
    public abstract class GroupedReport<TKey, TElement> : Report
    {
        public GroupedReport(string title, IEnumerable<IGrouping<TKey, TElement>> data, 
            string subtitle = null, ReportFormat reportFormat = ReportFormat.Text, string destinationFile = null)
            : base(title, subtitle, reportFormat, destinationFile)
	    {
            data.ThrowIfNull("Argument null: data");

            Data = data;
	    }

        public IEnumerable<IGrouping<TKey, TElement>> Data { get; protected set; }

        public override bool IsGroupedReport
        {
            get
            {
                return true;
            }
        }
    }
}