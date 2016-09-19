using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using Wintellect.Resources;

namespace Wintellect.NOW
{
    public class VideoActivityGroupedByEmailPDFReport : GroupedPDFReport<string, VideoAndCompletionPercentage>
    {
        public VideoActivityGroupedByEmailPDFReport(string reportTitle,
            IEnumerable<IGrouping<string, VideoAndCompletionPercentage>> data,
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

            foreach(var group in Data)
            {
                currentGroup++;

                PdfPTable table = new PdfPTable(3);

                float[] widths = new float[] { 1f, 10f, 4f };
                table.TotalWidth = _settings.PageSize.Width * 0.8f;
                table.LockedWidth = true;
                table.SetWidths(widths);

                var itemCount = group.LongCount();
                long currentItem = 0;
                var numVideosLabel = string.Format("{0} {1}", itemCount, itemCount == 1 ? "video" : "videos");
                PdfPCell cell = new PdfPCell(new Phrase(string.Format("{0}. {1} ({2})", currentGroup, group.Key, numVideosLabel), groupKeyFont));
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
                    table.AddCell(item.Video);
                    table.AddCell(string.Format("{0,3}%", item.CompletionPercentage));
                }

                _document.Add(table);

                if (currentGroup < groupCount) _document.Add(new Paragraph(string.Concat<string>(Enumerable.Repeat(Environment.NewLine, 2))));
            }
        }
    }
}
