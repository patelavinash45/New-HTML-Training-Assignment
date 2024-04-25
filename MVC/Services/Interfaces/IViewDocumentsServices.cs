using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IViewDocumentsServices
    {
        ViewDocument GetDocumentList(int requestId, int aspNetUserId);

        Task<bool> UploadFile(ViewDocument model, String firstName, String lastName, int requestId);

        Task<bool> DeleteFile(int requestWiseFileId);

        Task<bool> DeleteAllFile(String requestWiseFileIds, int requestId);

        bool SendFileMail(String requestWiseFileIds, int requestId);

    }
}
