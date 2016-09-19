using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Configuration;
using System.IO;
using Wintellect.Resources;
using Wintellect.Extensions;
using Wintellect.NOW.Configuration;

namespace Wintellect.NOW
{
    public class PDFReportSettings
    {
        public static readonly PDFReportSettings Default;

        static PDFReportSettings()
        {
            Default = new PDFReportSettings();

            Default.ReportTitle = "Report";
            Default.ReportSubtitle = null;
            Default.IsGroupedReport = false;
            Default.Logo = TryGetLogoFromConfigurationFile();
            Default.PageNumberContainer = TryGetPageNumberContainerImageFromConfigurationFile();
            Default.PageSize = iTextSharp.text.PageSize.A4;
            Default.MarginLeft = 40f;
            Default.MarginRight = 80f;
            Default.MarginTop = 100f;
            Default.MarginBottom = 80f;

            Default.ReportTitleFont = SetFont(FontNames.SegoeUILight, 22f, 85, 100, 226);
            Default.ReportSubTitleFont = SetFont(FontNames.SegoeUILight, 14f, 255, 255, 255, FontStyle.Italic);
            Default.ReportFooterFont = SetFont(FontNames.SegoeUILight, 11f, 245, 5, 53);
            Default.ReportTimestampFont = SetFont(FontNames.SegoeUILight, 10f, 128, 128, 128);
        }

        public static Image TryGetLogoFromConfigurationFile()
        {
            // TO DO: make this configurable
            try
            {
                var path = ConfigurationManager.GetSection("Wintellect.NOW") as WintellectNOWSettings;

                if (path.IsNotNull() && path.Reporting.IsNotNull() && path.Reporting.Logo.IsNotNull())
                {
                    var image = Image.GetInstance(new FileInfo(path.Reporting.Logo).FullName);

                    return image;
                }

                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static Image TryGetPageNumberContainerImageFromConfigurationFile()
        {
            try
            {
                var path = ConfigurationManager.GetSection("Wintellect.NOW") as WintellectNOWSettings;

                if (path.IsNotNull() && path.Reporting.IsNotNull() && path.Reporting.PageNumberContainerImage.IsNotNull())
                {
                    var image = Image.GetInstance(new FileInfo(path.Reporting.PageNumberContainerImage).FullName);

                    return image;
                }

                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public PDFReportSettings(string title = null, string subtitle = null,  bool? isGrouped = null)
        {
            RegisterDefaultFontDirectoryFromConfigurationFile();

            if (Default != null)
            {
                ReportTitle = title ?? Default.ReportTitle;
                ReportSubtitle = subtitle ?? Default.ReportSubtitle;
                IsGroupedReport = isGrouped.HasValue ? isGrouped.Value : Default.IsGroupedReport;
                PageSize = Default.PageSize;
                MarginLeft = Default.MarginLeft;
                MarginRight = Default.MarginRight;
                MarginTop = Default.MarginTop;
                MarginBottom = Default.MarginBottom;
                Logo = Default.Logo;
                PageNumberContainer = Default.PageNumberContainer;
                ReportTitleFont = Default.ReportTitleFont;
                ReportSubTitleFont = Default.ReportSubTitleFont;
                ReportFooterFont = Default.ReportFooterFont;
                ReportTimestampFont = Default.ReportTimestampFont;
            }
        }

        private void RegisterDefaultFontDirectoryFromConfigurationFile()
        {
            // TO DO: make this configurable

            FontFactory.RegisterDirectory("C:\\Windows\\Fonts");
        }

        public string ReportTitle { get; set; }
        public string ReportSubtitle { get; set; }
        public bool IsGroupedReport { get; set; }

        public Rectangle PageSize { get; set; }
        public float MarginLeft { get; set;}
        public float MarginRight { get; set; }
        public float MarginTop { get; set; }
        public float MarginBottom { get; set;}

        public Font ReportTitleFont = null;
        public Font ReportSubTitleFont = null;
        public Font ReportFooterFont = null;
        public Font ReportTimestampFont = null;

        public Image Logo = null;
        public Image PageNumberContainer = null;

        public string DefaultFontDirectory { get; private set; }

        public ReportFormat ReportFormat { get { return NOW.ReportFormat.PDF; } }

        public static Font SetFont(string fontName, float size, int colorR, int colorG, int colorB, FontStyle style = FontStyle.Normal)
        {
            var font = FontFactory.GetFont(FontNames.SegoeUILight, size);
            font.SetColor(colorR, colorG, colorB);
            font.SetStyle((int)style);

            return font;
        }
    }
}