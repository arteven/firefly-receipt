using System.Threading.Tasks;

namespace Gevlee.FireflyReceipt.Application.Services
{
    public interface IReceiptImageProvider
    {
        Task<(byte[] imageBytes, string mimeType)> LoadReceiptAsync(string filePath);
    }
}
