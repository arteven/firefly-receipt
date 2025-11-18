using Gevlee.FireflyReceipt.Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gevlee.FireflyReceipt.Application.Services
{
    public interface IAttachmentService
    {
        Task<IEnumerable<AlreadyAssignedReceipt>> GetAlreadyAssignedReceipts();
        Task<bool> AssignReceipt(string imgPath, long transactionId);
    }
}