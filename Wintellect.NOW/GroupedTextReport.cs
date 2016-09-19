using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Wintellect.NOW
{
    public class GroupedTextReport<TKey, TElement> : GroupedReport<TKey, TElement>
    {
        protected IndentedTextWriter _indentedTextWriter;

        public GroupedTextReport(string title,
            IEnumerable<IGrouping<TKey, TElement>> data, 
            string subtitle = null, string destinationFile = null)
            : base(title, data, subtitle, ReportFormat.Text, destinationFile)
        {
            _writer = new IndentedTextWriter(new StreamWriter(destinationFile, false));

            _indentedTextWriter = _writer as IndentedTextWriter;

            Write();
        }

        public override void Write()
        {
            _indentedTextWriter.WriteLine(Title);
            _indentedTextWriter.WriteLine(Subtitle);

            _indentedTextWriter.WriteLine();

            foreach (var group in Data)
            {
                _indentedTextWriter.WriteLine(group.Key);

                _indentedTextWriter.Indent++;

                foreach (var item in group)
                    _indentedTextWriter.WriteLine(item);

                _indentedTextWriter.Indent--;
            }
        }
    }
}