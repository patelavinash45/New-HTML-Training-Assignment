using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IRequestStatusLogRepository
    {
        List<RequestStatusLog> GetRequestStatusLogByRequestId(int requestId);

        Task<bool> AddRequestSatatusLog(RequestStatusLog requestStatusLog);

        Task<bool> AddBlockRequest(BlockRequest blockRequest);
    }
}
