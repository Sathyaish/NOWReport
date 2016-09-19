using System.Collections.Generic;
using System.IO;

namespace Wintellect.NOW
{
    public class FlatTextReport<T> : FlatReport<T>
    {
        public FlatTextReport(string title, IEnumerable<T> data, 
            string subtitle = null, string destinationFile = null)
            : base(title, data, subtitle, ReportFormat.Text, destinationFile)
        {
            _writer = new StreamWriter(destinationFile, false);

            Write();
        }

        public override void Write()
        {
            _writer.WriteLine(Title);

            _writer.WriteLine();

            foreach (var item in Data)
            {
                _writer.WriteLine(item);
            }
        }
    }
}
