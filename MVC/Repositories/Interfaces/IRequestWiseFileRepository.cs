using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IRequestWiseFileRepository
    {
        Task<bool> addFile(RequestWiseFile requestWiseFile);

        List<RequestWiseFile> getFilesByrequestId(int requestId);
        
        List<RequestWiseFile> getRequestWiseFilesByIds(List<int> ids);

        RequestWiseFile getFilesByrequestWiseFileId(int requestWiseFileId);

        Task<bool> updateRequestWiseFiles(List<RequestWiseFile> requestWiseFile);
    }
}
