using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Diagnostics;
using System.IO;

namespace TextSharpExample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                FontFactory.Register("C:\\Windows\\Fonts\\segoeuil.ttf", "Segoe UI Light");

                using (var document = new Document(PageSize.A4, 72, 72, 72, 72))
                {
                    using (var writer = PdfWriter.GetInstance(document, new FileStream("C:\\temp\\Foo.pdf", FileMode.Create)))
                    {
                        writer.PageEvent = new PageEventHandler();

                        document.Open();

                        document.AddHeader("defaultHeader", "This is the header of this document.");

                        document.AddTitle("This is the document title");

                        var headingFont = FontFactory.GetFont("Segoe UI Light", 20f, BaseColor.GREEN);
                        var runningTextFont = FontFactory.GetFont("Segoe UI Light", 11f, BaseColor.DARK_GRAY);

                        var text = File.ReadAllText("C:\\temp\\source.txt");

                        document.Add(new Paragraph("This is a paragraph.", headingFont));

                        FontFactory.Register("C:\\Windows\\Fonts\\georgia.ttf", "Georgia");
                        var georgia = FontFactory.GetFont("Georgia", 20f);

                        var chunk = new Chunk("This is a chunk", georgia);
                        chunk.SetBackground(BaseColor.GREEN);
                        // chunk.SetHorizontalScaling(0.7f);
                        chunk.setLineHeight(2f);
                        // chunk.SetSkew(0f, 10f);
                        // chunk.SetTextRenderMode(1, 0.5f, BaseColor.BLUE);
                        chunk.SetTextRise(5f);
                        chunk.SetWordSpacing(1f);
                        document.Add(chunk);
                        
                        document.Add(new Paragraph(text, runningTextFont));

                        document.Close();
                    }
                }

                var process = Process.Start(new ProcessStartInfo("C:\\temp\\Foo.pdf"));
                process.Close();
            }
            catch (Exception ex)
            {
                Debugger.Break();

                Debug.Print(ex.ToString());
            }
        }
    }

    public class PageEventHandler : PdfPageEventHelper
    {
        PdfTemplate pdfTemplate;
        PdfContentByte pdfContentByte;

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);

            pdfContentByte = writer.DirectContent;

            pdfTemplate = pdfContentByte.CreateTemplate(50, 50);
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);

            Rectangle pageSize = document.PageSize;

            pdfContentByte.BeginText();
            pdfContentByte.SetFontAndSize(BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 15f);
            pdfContentByte.SetRGBColorFill(50, 50, 200);
            pdfContentByte.SetTextMatrix(pageSize.GetLeft(10), pageSize.GetTop(10));
            pdfContentByte.ShowText("This is the title I wrote on start page event.");
            pdfContentByte.EndText();

            PdfPTable pdfPTableHeaderTable = new PdfPTable(2);
            pdfPTableHeaderTable.TotalWidth = pageSize.Width - 80;
            pdfPTableHeaderTable.SetWidthPercentage(new float[] { 45, 45 }, pageSize);

            pdfPTableHeaderTable.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfPTableHeaderTable.DefaultCell.AddElement(new Phrase("Default cell text"));

            PdfPCell pdfPCellLeft = new PdfPCell(new Phrase(8, "Header on the left", new Font(Font.FontFamily.TIMES_ROMAN, 10f)));
            pdfPCellLeft.Padding = 5;
            pdfPCellLeft.PaddingBottom = 8;
            // pdfPCellLeft.BorderWidthRight = 0;
            pdfPCellLeft.Border = 0;
            pdfPTableHeaderTable.AddCell(pdfPCellLeft);

            PdfPCell pdfCellRight = new PdfPCell(new Phrase(8, "Header on the right", new Font(Font.FontFamily.HELVETICA, 10f, 0, BaseColor.GREEN)));
            pdfCellRight.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            pdfCellRight.Padding = 5;
            pdfCellRight.PaddingBottom = 8;
            // pdfCellRight.BorderWidthLeft = 0;
            pdfCellRight.Border = 0;
            pdfPTableHeaderTable.AddCell(pdfCellRight);

            pdfContentByte.SetRGBColorFill(0, 0, 100);
            pdfContentByte.SetRGBColorStroke(200, 100, 9);

            pdfPTableHeaderTable.WriteSelectedRows(0, -1, pageSize.GetLeft(40), pageSize.GetTop(50), pdfContentByte);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            var currentPageNumber = writer.PageNumber;

            Rectangle pageSize = document.PageSize;
            var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            var pageNumberTextFormat = "Page {0} of ";
            var lengthOfPageNumberTextInBaseFont = baseFont.GetWidthPoint(pageNumberTextFormat, 10f);

            pdfContentByte.BeginText();
            pdfContentByte.SetRGBColorFill(255, 100, 200);
            pdfContentByte.SetFontAndSize(baseFont, 10f);
            pdfContentByte.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetBottom(30));
            pdfContentByte.ShowText(string.Format(pageNumberTextFormat, currentPageNumber));
            pdfContentByte.EndText();

            pdfContentByte.AddTemplate(pdfTemplate, pageSize.GetLeft(40) + lengthOfPageNumberTextInBaseFont, pageSize.GetBottom(30));

            pdfContentByte.AddImage(Image.GetInstance("C:\\temp\\Wintellect.png"), 80f, 180f, 0f, 38f, 250f, 20f);

            pdfContentByte.BeginText();
            pdfContentByte.SetFontAndSize(baseFont, 10f);
            pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT,
                "Printed On " + DateTime.Now.ToString(), pageSize.GetRight(40), pageSize.GetBottom(30), 0);
            pdfContentByte.EndText();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            base.OnCloseDocument(writer, document);

            pdfTemplate.BeginText();
            pdfTemplate.SetFontAndSize(baseFont, 10f);
            pdfTemplate.SetTextMatrix(0, 0);
            pdfTemplate.ShowText((writer.PageNumber - 1).ToString());
            pdfTemplate.EndText();
        }
    }
}