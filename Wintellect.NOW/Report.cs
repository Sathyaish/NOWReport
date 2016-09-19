using System;
using System.IO;
using Wintellect.Extensions;

namespace Wintellect.NOW
{
    public abstract class Report : IDisposable
    {
        protected TextWriter _writer = null;

        public Report(string title, string subTitle = null, ReportFormat reportFormat = ReportFormat.Text, string destinationFile = null)
        {
            title.IsNullOrEmpty().ThrowIfTrue("Argument null or empty string: title");

            Title = title;

            Subtitle = subTitle;

            ReportFormat  = reportFormat;

            DestinationFile = destinationFile ?? GetTempFileName();
        }

        protected virtual string GetTempFileName()
        {
            // TO DO:
            throw new NotImplementedException();
        }

        public string Title { get; protected set; }

        public string Subtitle { get; protected set; }

        public ReportFormat ReportFormat { get; protected set; }

        public string DestinationFile { get; protected set; }

        public abstract bool IsGroupedReport { get; }

        public abstract void Write();

        public virtual void Dispose()
        {
            if (_writer.IsNotNull())
            {
                _writer.Flush();

                _writer.Dispose();
            }
        }
    }
}