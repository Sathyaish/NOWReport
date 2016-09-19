using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wintellect.Resources;

namespace Wintellect.NOW
{
    public abstract class GroupedPDFReport<TKey, TElement> : GroupedReport<TKey, TElement>
    {
        protected PDFReportSettings _settings = null;

        protected Document _document = null;
        protected PdfWriter _pdfWriter = null;
        
        public GroupedPDFReport(string reportTitle,
            IEnumerable<IGrouping<TKey, TElement>> data, 
            string subtitle = null, string destinationFile = null, PDFReportSettings settings = null)
            : base(reportTitle, data, subtitle, ReportFormat.PDF, destinationFile)
        {
            _settings = settings ?? new PDFReportSettings(reportTitle, subtitle, true);

            _document = new Document(_settings.PageSize, _settings.MarginLeft, _settings.MarginRight, _settings.MarginTop, _settings.MarginBottom);
            
            _pdfWriter = PdfWriter.GetInstance(_document, new FileStream(DestinationFile, FileMode.Create));

            _pdfWriter.PageEvent = new PDFReportPageEventHandler(_settings);
            
            _document.Open();

            Write();
        }

        public override void Dispose()
        {
            // This will close all associated document 
            // writers/listeners such as the PdfWriter
            if (_document != null) _document.Dispose();

            base.Dispose();
        }
    }
}