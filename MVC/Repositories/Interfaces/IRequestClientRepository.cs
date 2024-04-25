using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IRequestClientRepository
    {
        List<Region> GetAllRegions();

        List<CaseTag> GetAllReason();

        List<RequestClient> GetAllRequestClients();

        List<RequestClient> GetRequestClientsBasedOnFilter(Func<RequestClient,bool> predicate);

        List<BlockRequest> GetRequestClientsAndBlockRequestBasedOnFilter(Func<BlockRequest, bool> predicate);

        int CountRequestClientsAndBlockRequestBasedOnFilter(Func<BlockRequest, bool> predicate);

        List<RequestClient> GetRequestClientByStatus(Func<RequestClient, bool> predicate, int skip);

        int CountRequestClientByStatusForAdmin(List<int> status);

        int CountRequestClientByStatusForPhysician(List<int> status, int aspNetUserId);

        int CountRequestClientByStatusAndFilter(Func<RequestClient, bool> predicate);

        List<RequestClient> GetAllRequestClientForUser(int userId);

        Task<bool> AddRequestClient(RequestClient requestClient);

        RequestClient GetRequestClientByRequestId(int requestId);

        RequestClient GetRequestClientAndRequestByRequestId(int requestId);

        Task<bool> UpdateRequestClient(RequestClient requestClient);

        List<RequestClient> GetAllRequestClientsByUserId(int userId, int skip);
        
        int CountRequestClientsByUserId(int userId);

        Task<bool> DeleteBlockRequest(int requestId);
    }
}