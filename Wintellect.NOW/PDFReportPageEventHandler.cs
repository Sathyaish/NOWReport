using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using Wintellect.Extensions;

namespace Wintellect.NOW
{
    public class PDFReportPageEventHandler : PdfPageEventHelper
    {
        protected PdfContentByte _pdfContentByte = null;
        protected PDFReportSettings _settings = null;
        protected PdfTemplate _numPagesTemplate = null;

        protected float _numPagesTemplateLeft;
        protected float _numPagesTemplateTop;

        public PDFReportPageEventHandler(PDFReportSettings settings = null)
        { 
            _settings = settings ?? PDFReportSettings.Default;
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);

            _pdfContentByte = writer.DirectContent;

            _numPagesTemplate = _pdfContentByte.CreateTemplate(_settings.ReportFooterFont.BaseFont.GetWidthPoint("    ", _settings.ReportFooterFont.Size), _settings.ReportFooterFont.Size);
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);

            DrawPageBorder(writer, document);

            WriteReportTitle(writer, document);

            WriteReportSubtitle(writer, document);

            DrawLogo(writer, document);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            WriteReportTimestamp(writer, document);

            WriteCurrentPageNumber(writer, document);
        }

        private void WriteReportTimestamp(PdfWriter writer, Document document)
        {
            var left = document.GetLeft(0);
            var top = 22;
            
            var pdfContentByteUnder = writer.DirectContentUnder;

            _pdfContentByte.BeginText();
            _pdfContentByte.SetFontAndSize(_settings.ReportTimestampFont.BaseFont, _settings.ReportTimestampFont.Size);
            _pdfContentByte.SetRGBColorFill(_settings.ReportTimestampFont.Color.R, _settings.ReportTimestampFont.Color.G, _settings.ReportTimestampFont.Color.B);
            _pdfContentByte.SetTextMatrix(left, top);
            _pdfContentByte.ShowText(DateTime.Now.ToUniversalTime().ToString("u"));
            _pdfContentByte.EndText();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            WriteTotalNumberOfPages(writer, document);
        }

        private void WriteTotalNumberOfPages(PdfWriter writer, Document document)
        {
            var width = _settings.ReportFooterFont.BaseFont.GetWidthPoint("    ", _settings.ReportFooterFont.Size);
            var height = _settings.ReportSubTitleFont.Size;

            var pdfContentByteUnder = writer.DirectContentUnder;

            _numPagesTemplate.BeginText();
            _numPagesTemplate.SetFontAndSize(_settings.ReportFooterFont.BaseFont, _settings.ReportFooterFont.Size);
            _numPagesTemplate.SetRGBColorFill(_settings.ReportFooterFont.Color.R, _settings.ReportFooterFont.Color.G, _settings.ReportFooterFont.Color.B);
            _numPagesTemplate.SetTextMatrix(0, 0);
            _numPagesTemplate.ShowText((writer.PageNumber - 1).ToString());
            _numPagesTemplate.EndText();
        }

        protected virtual void WriteCurrentPageNumber(PdfWriter writer, Document document)
        {
            var left = _settings.PageSize.Width - 100;
            var top = 22;
            var width = _settings.ReportFooterFont.BaseFont.GetWidthPoint("    ", _settings.ReportFooterFont.Size);
            var height = _settings.ReportSubTitleFont.Size;

            var pdfContentByteUnder = writer.DirectContentUnder;

            _pdfContentByte.BeginText();
            _pdfContentByte.SetFontAndSize(_settings.ReportFooterFont.BaseFont, _settings.ReportFooterFont.Size);
            _pdfContentByte.SetRGBColorFill(_settings.ReportFooterFont.Color.R, _settings.ReportFooterFont.Color.G, _settings.ReportFooterFont.Color.B);
            _pdfContentByte.SetTextMatrix(left, top);
            _pdfContentByte.ShowText(writer.PageNumber.ToString());
            _pdfContentByte.EndText();

            if (_settings.PageNumberContainer != null)
            {
                var imgLeft = left - _settings.PageNumberContainer.Width / 2 + 5;
                var imgTop = top - _settings.PageNumberContainer.Height / 2;

                _settings.PageNumberContainer.SetAbsolutePosition(imgLeft, imgTop);
                pdfContentByteUnder.SaveState();
                pdfContentByteUnder.AddImage(_settings.PageNumberContainer);
                pdfContentByteUnder.RestoreState();
            }

            left = left + (_settings.PageNumberContainer == null ? 0 : (_settings.PageNumberContainer.Width / 2)) + 5;

            _pdfContentByte.BeginText();
            _pdfContentByte.SetFontAndSize(_settings.ReportFooterFont.BaseFont, _settings.ReportFooterFont.Size);
            _pdfContentByte.SetRGBColorFill(_settings.ReportFooterFont.Color.R, _settings.ReportFooterFont.Color.G, _settings.ReportFooterFont.Color.B);
            _pdfContentByte.SetTextMatrix(left, top);
            _pdfContentByte.ShowText(" of ");
            _pdfContentByte.EndText();


            _numPagesTemplateLeft = left + 15 + (_settings.PageNumberContainer == null ? 0 :_settings.PageNumberContainer.Width / 2);
            _numPagesTemplateTop = top;

            _pdfContentByte.AddTemplate(_numPagesTemplate, _numPagesTemplateLeft, _numPagesTemplateTop);

            if (_settings.PageNumberContainer != null)
            {
                var imgLeft = _numPagesTemplateLeft - _settings.PageNumberContainer.Width / 2 + 8;
                var imgTop = _numPagesTemplateTop - _settings.PageNumberContainer.Height / 2;

                var totalPagesImage = Image.GetInstance(_settings.PageNumberContainer);
                totalPagesImage.SetAbsolutePosition(imgLeft, imgTop);
                pdfContentByteUnder.SaveState();
                pdfContentByteUnder.AddImage(totalPagesImage);
                pdfContentByteUnder.RestoreState();
            }
        }

        protected virtual void DrawLogo(PdfWriter writer, Document document)
        {
            // TO DO
            if (_settings.Logo.IsNotNull())
            {
                _settings.Logo.SetAbsolutePosition(_settings.PageSize.Width - 50, _settings.PageSize.Height / 4);

                _pdfContentByte.AddImage(_settings.Logo);
            }
        }

        protected virtual void WriteReportSubtitle(PdfWriter writer, Document document)
        {
            if (!_settings.ReportSubtitle.IsNullOrEmpty())
            {
                var left = _settings.PageSize.Width - 150;
                var top = _settings.PageSize.Top - (40 + _settings.ReportTitleFont.Size + 10);
                var width = _settings.ReportSubTitleFont.BaseFont.GetWidthPoint(_settings.ReportSubtitle.Plus("  "), _settings.ReportSubTitleFont.Size);
                var height = _settings.ReportSubTitleFont.Size;

                _pdfContentByte.BeginText();
                _pdfContentByte.SetFontAndSize(_settings.ReportSubTitleFont.BaseFont, _settings.ReportSubTitleFont.Size);
                _pdfContentByte.SetRGBColorFill(_settings.ReportSubTitleFont.Color.R, _settings.ReportSubTitleFont.Color.G, _settings.ReportSubTitleFont.Color.B);
                _pdfContentByte.SetTextMatrix(1, 0, 0.25f, 1, left, top);
                _pdfContentByte.ShowText(_settings.ReportSubtitle);
                _pdfContentByte.EndText();

                var pdfContentByteUnder = writer.DirectContentUnder;
                pdfContentByteUnder.SaveState();
                pdfContentByteUnder.SetRGBColorFill(255, 0, 0);
                pdfContentByteUnder.Rectangle(left - 2, top - 4, width + 2, height + 6);
                pdfContentByteUnder.Fill();
                pdfContentByteUnder.RestoreState();
            }
        }

        protected virtual void WriteReportTitle(PdfWriter writer, Document document)
        {
            if (!_settings.ReportTitle.IsNullOrEmpty())
            {
                _pdfContentByte.BeginText();
                _pdfContentByte.SetFontAndSize(_settings.ReportTitleFont.BaseFont, _settings.ReportTitleFont.Size);
                _pdfContentByte.SetRGBColorFill(_settings.ReportTitleFont.Color.R, _settings.ReportTitleFont.Color.G, _settings.ReportTitleFont.Color.B);
                _pdfContentByte.SetTextMatrix(_settings.PageSize.Width - 220, _settings.PageSize.Top - 40);
                _pdfContentByte.ShowText(_settings.ReportTitle);
                _pdfContentByte.EndText();
            }
        }

        protected virtual void DrawPageBorder(PdfWriter writer, Document document)
        {
            _pdfContentByte.SetRGBColorStroke(84, 155, 226);
            _pdfContentByte.SetLineWidth(0.75f);
            _pdfContentByte.SetLineDash(2f, 10f);
            _pdfContentByte.Rectangle(5f, 5f, _settings.PageSize.Width - 10, _settings.PageSize.Height - 10);
            _pdfContentByte.Stroke();
        }
    }
}