using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IRequestWiseFileRepository
    {
        Task<bool> AddFile(RequestWiseFile requestWiseFile);

        List<RequestWiseFile> GetFilesByRequestId(int requestId);
        
        List<RequestWiseFile> GetRequestWiseFilesByIds(List<int> ids);

        RequestWiseFile GetFilesByRequestWiseFileId(int requestWiseFileId);

        Task<bool> UpdateRequestWiseFiles(List<RequestWiseFile> requestWiseFile);
    }
}
