using Repositories.DataContext;
using Repositories.DataModels;
using Repositories.Interfaces;

namespace Repositories.Implementation
{
    public class RequestStatusLogRepository : IRequestStatusLogRepository
    {
        private readonly HalloDocDbContext _dbContext;

        public RequestStatusLogRepository(HalloDocDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<RequestStatusLog> GetRequestStatusLogByRequestId(int requestId)
        {
            return _dbContext.RequestStatusLogs.Where(a => a.RequestId == requestId).ToList();
        }

        public async Task<bool> AddRequestSatatusLog(RequestStatusLog requestStatusLog)
        {
            _dbContext.RequestStatusLogs.Add(requestStatusLog);
            return await _dbContext.SaveChangesAsync() > 0;
        }  

        public async Task<bool> AddBlockRequest(BlockRequest blockRequest)
        {
            _dbContext.BlockRequests.Add(blockRequest);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
