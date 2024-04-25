using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IRequestWiseFileRepository
    {
        Task<bool> AddFile(RequestWiseFile requestWiseFile);

        List<RequestWiseFile> GetFilesByrequestId(int requestId);
        
        List<RequestWiseFile> GetRequestWiseFilesByIds(List<int> ids);

        RequestWiseFile GetFilesByrequestWiseFileId(int requestWiseFileId);

        Task<bool> UpdateRequestWiseFiles(List<RequestWiseFile> requestWiseFile);
    }
}
