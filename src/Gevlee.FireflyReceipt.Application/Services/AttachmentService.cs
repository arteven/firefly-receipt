using Gevlee.FireflyReceipt.Application.Models;
using Gevlee.FireflyReceipt.Application.Models.Firefly;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gevlee.FireflyReceipt.Application.Services
{
    public class AttachmentService : IAttachmentService
    {
        public AttachmentService(IFireflyClient client)
        {
            Client = client;
        }

        private IFireflyClient Client { get; }

        public async Task<IEnumerable<AlreadyAssignedReceipt>> GetAlreadyAssignedReceipts()
        {
            var result = new List<AlreadyAssignedReceipt>();
            var page = 1;
            var lastPage = false;

            while (!lastPage)
            {
                var response = await Client.GetAttachmentsAsync(page);
                result.AddRange(response.Data
                    .Where(x => x.Attributes.AttachableType == "TransactionJournal")
                    .Select(
                    x => new AlreadyAssignedReceipt
                    {
                        Filename = x.Attributes.Filename,
                        TransactionId = int.Parse(x.Attributes.AttachableId)
                    }));
                lastPage = response.Meta.Pagination.TotalPages <= page;
                page++;
            }

            return result;
        }

        public async Task AssignReceipt(string imgPath, long transactionId)
        {
            var fileName = System.IO.Path.GetFileName(imgPath);
            var createAttachmentResponse = await Client.CreateAttachmentAsync(new CreateAttachmentRequest
            {
                AttachableType = AttachableType.TransactionJournal,
                AttachableId = transactionId.ToString(),
                Filename = fileName
            });

            await Client.UploadAttachment(createAttachmentResponse.Data.Id, File.ReadAllBytes(imgPath));
        }
    }
}
