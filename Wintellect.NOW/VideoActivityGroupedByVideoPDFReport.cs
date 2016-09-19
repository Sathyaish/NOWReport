using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.Resources;

namespace Wintellect.NOW
{
    public class VideoActivityGroupedByVideoPDFReport : GroupedPDFReport<string, EmailAndCompletionPercentage>
    {
        public VideoActivityGroupedByVideoPDFReport(string reportTitle,
            IEnumerable<IGrouping<string, EmailAndCompletionPercentage>> data,
            string subtitle = null, string destinationFile = null, PDFReportSettings settings = null)
            : base(reportTitle, data, subtitle, destinationFile, settings)
        {
        }

        public override void Write()
        {
            var reportTitleFont = _settings.ReportTitleFont;

            var groupKeyFont = FontFactory.GetFont(FontNames.SegoeUILight, 14f);
            groupKeyFont.SetColor(reportTitleFont.Color.R, reportTitleFont.Color.G, reportTitleFont.Color.B);

            long groupCount = Data.LongCount();
            long currentGroup = 0;

            foreach (var group in Data)
            {
                currentGroup++;

                PdfPTable table = new PdfPTable(3);

                float[] widths = new float[] { 1f, 10f, 5f };
                table.TotalWidth = _settings.PageSize.Width * 0.8f;
                table.LockedWidth = true;
                table.SetWidths(widths);

                var itemCount = group.LongCount();
                long currentItem = 0;
                var numViewersLabel = string.Format("{0} {1}", itemCount, itemCount == 1 ? "viewer" : "viewers");
                PdfPCell cell = new PdfPCell(new Phrase(string.Format("{0}. {1} ({2})", currentGroup, group.Key, numViewersLabel), groupKeyFont));
                cell.Colspan = 3;
                cell.BorderWidth = 0;
                cell.HorizontalAlignment = 0;
                table.AddCell(cell);

                foreach (var item in group)
                {
                    PdfPCell serialNumberColumn = new PdfPCell(new Phrase(string.Format("{0}. ", ++currentItem)));
                    serialNumberColumn.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    serialNumberColumn.BorderWidth = 0;
                    table.AddCell(serialNumberColumn);

                    table.DefaultCell.BorderWidth = 0;
                    table.AddCell(item.Email);
                    table.AddCell(string.Format("{0,3}%", item.CompletionPercentage));
                }

                _document.Add(table);

                if (currentGroup < groupCount) _document.Add(new Paragraph(string.Concat<string>(Enumerable.Repeat(Environment.NewLine, 2))));
            }
        }
    }
}