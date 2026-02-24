using Peso_Baseed_Barcode_Printing_System_API.Interface;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Text;
using System.Text.RegularExpressions;

namespace Peso_Based_Barcode_Printing_System_API.Services
{
    public class PdfReaderService
    {
        // This method processes the PDF stream and extracts text from all pages
        public string ReadPdfFromStream(Stream pdfStream)
        {
            StringBuilder text = new StringBuilder();

            // Ensure the stream position is at the start
            pdfStream.Position = 0;

            // Check if the stream is empty
            if (pdfStream.Length == 0)
            {
                throw new IOException("The PDF stream is empty.");
            }

            try
            {
                // Open the PDF document from the memory stream
                using (PdfReader reader = new PdfReader(pdfStream))
                {
                    using (PdfDocument pdfDoc = new PdfDocument(reader))
                    {
                        // Loop through all pages in the document
                        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                        {
                            // Extract text from each page
                            string pageText = ExtractTextFromPage(pdfDoc, i);
                            text.Append(pageText);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                throw new IOException("Error reading PDF file. Make sure the file is a valid PDF.", ex);
            }

            return text.ToString();
        }

        // This method extracts text from a specific page using iText 7's PdfCanvasProcessor
       /* private string ExtractTextFromPage(PdfDocument pdfDoc, int pageNumber)
        {
            StringBuilder pageText = new StringBuilder();

            // Extract text from the page using PdfCanvasProcessor
            var page = pdfDoc.GetPage(pageNumber);
            var strategy = new SimpleTextExtractionStrategy();
            string extractedText = PdfTextExtractor.GetTextFromPage(page, strategy);

            pageText.Append(extractedText.Trim());
            return pageText.ToString().Trim();
        }*/

        private string ExtractTextFromPage(PdfDocument pdfDoc, int pageNumber)
        {
            var page = pdfDoc.GetPage(pageNumber);
            var strategy = new SimpleTextExtractionStrategy();
            string extractedText = PdfTextExtractor.GetTextFromPage(page, strategy);

            // Remove line breaks, carriage returns, tabs
            string cleaned = extractedText
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\t", "")
                .Trim();

            // Optional: Replace multiple spaces with single space
            cleaned = Regex.Replace(cleaned, @"\s{2,}", " ");

            return cleaned;
        }


    }
}

