using Microsoft.AspNetCore.Http;
using Services.ViewModels.Admin;

namespace Services.Interfaces.AdminServices
{
    public interface IViewNotesService
    {
        ViewNotes GetNotes(int RequestId);

        Task<bool> AddAdminNotes(String newNotes, int requestId, int aspNetUserId, bool isAdmin);

        Task<bool> CancelRequest(CancelPopUp model);

        Task<bool> AssignRequest(AssignAndTransferPopUp model);

        Task<bool> BlockRequest(BlockPopUp model);

        Task<bool> ClearRequest(int requestId);

        bool SendAgreement(Agreement model,HttpContext httpContext);

        Task<bool> AgreementDeclined(Agreement model);

        Task<bool> AgreementAgree(Agreement model);
    }
}
