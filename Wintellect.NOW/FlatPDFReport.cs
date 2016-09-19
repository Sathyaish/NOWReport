using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using Wintellect.Resources;
using Wintellect.Extensions;

namespace Wintellect.NOW
{
    public class FlatPDFReport<T> : FlatReport<T>
    {
        protected PDFReportSettings _settings = null;
        protected Document _document = null;
        protected PdfWriter _pdfWriter = null;

        public FlatPDFReport(string reportTitle, IEnumerable<T> data, 
            string subtitle = null, string destinationFile = null, PDFReportSettings settings = null)
            : base(reportTitle, data, subtitle, ReportFormat.Text, destinationFile)
        {
            _settings = settings ?? new PDFReportSettings(reportTitle, subtitle);

            _document = new Document(_settings.PageSize, _settings.MarginLeft, _settings.MarginRight, _settings.MarginTop, _settings.MarginBottom);

            _pdfWriter = PdfWriter.GetInstance(_document, new FileStream(DestinationFile, FileMode.Create));

            _pdfWriter.PageEvent = new PDFReportPageEventHandler(_settings);

            _document.Open();

            Write();
        }

        public override void Write()
        {    
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            PdfPTable table = new PdfPTable(properties.Length);
            table.TotalWidth = _settings.PageSize.Width * 0.8f;
            table.LockedWidth = true;

            foreach(var property in properties)
            {
                table.AddCell(property.Name.ToTitleCase());
            }

            foreach(var item in Data)
            {
                foreach(var property in properties)
                {
                    var value = item.GetType().GetProperty(property.Name).GetValue(item);

                    table.AddCell(value.ToString());
                }
            }

            _document.Add(table);
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