using System;
using System.IO;
using System.Threading.Tasks;
using PDFtoImage;
using SkiaSharp;

namespace Gevlee.FireflyReceipt.Application.Services
{
    public class ReceiptImageProvider : IReceiptImageProvider
    {
        public async Task<(byte[] imageBytes, string mimeType)> LoadReceiptAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Receipt file not found: {filePath}", filePath);
            }

            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            if (extension == ".pdf")
            {
                return await ConvertPdfToPngAsync(filePath);
            }
            else
            {
                return await LoadImageAsync(filePath, extension);
            }
        }

        private Task<(byte[] imageBytes, string mimeType)> ConvertPdfToPngAsync(string pdfPath)
        {
            try
            {
                // Convert first page of PDF to SKBitmap
                using var pdfStream = File.OpenRead(pdfPath);
                using var bitmap = Conversion.ToImage(pdfStream, page: 0);

                // Convert SKBitmap to PNG bytes
                using var image = SKImage.FromBitmap(bitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);

                return Task.FromResult((data.ToArray(), "image/png"));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert PDF to image: {pdfPath}", ex);
            }
        }

        private async Task<(byte[] imageBytes, string mimeType)> LoadImageAsync(string imagePath, string extension)
        {
            var imageBytes = await File.ReadAllBytesAsync(imagePath);
            var mimeType = GetMimeType(extension);

            return (imageBytes, mimeType);
        }

        private string GetMimeType(string extension)
        {
            return extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "image/png" // Default fallback
            };
        }
    }
}
