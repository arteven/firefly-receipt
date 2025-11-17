using Gevlee.FireflyReceipt.Application.Models.Firefly;
using System;
using System.Threading.Tasks;

namespace Gevlee.FireflyReceipt.Application.Services
{
    public interface IFireflyClient
    {
        Task<GetAttachmentsResponse> GetAttachmentsAsync(int? page = null, int? limit = null);
        Task<GetTransactionsResponse> GetTransactionsAsync(int? page = null, int? limit = null, DateTime? start = null, DateTime? end = null, string type = null);
        Task<CreateAttachmentResponse> CreateAttachmentAsync(CreateAttachmentRequest requestModel);
        Task UploadAttachment(long attachmentId, byte[] fileBytes);
    }
}