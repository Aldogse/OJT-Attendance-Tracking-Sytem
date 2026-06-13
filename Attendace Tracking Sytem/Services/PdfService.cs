using DinkToPdf;
using DinkToPdf.Contracts;

namespace Attendace_Tracking_Sytem.Services
{
    public class PdfService
    {
        private readonly IConverter _converter;

        public PdfService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] GeneratePdf(string html)
        {
            var document = new HtmlToPdfDocument()
            {
                GlobalSettings =
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4
                },

                Objects =
                {
                    new ObjectSettings
                    {
                        HtmlContent =  html,
                        WebSettings =
                        {
                            DefaultEncoding = "utf-8"
                        }
                    }
                }
            };

            return _converter.Convert(document);
        }
    }
}
