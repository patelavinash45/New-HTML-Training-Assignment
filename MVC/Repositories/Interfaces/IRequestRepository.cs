using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IRequestRepository
    {
        Task<int> AddRequest(Request request);

        Request GetRequestByRequestId(int requestId);

        Task<bool> UpdateRequest(Request request);
    }
}
