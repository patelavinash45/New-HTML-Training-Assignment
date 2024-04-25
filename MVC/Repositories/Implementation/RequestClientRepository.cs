using Microsoft.EntityFrameworkCore;
using Repositories.DataContext;
using Repositories.DataModels;
using Repositories.Interfaces;
using System.Collections;

namespace Repositories.Implementation
{
    public class RequestClientRepository : IRequestClientRepository
    {
        private readonly HalloDocDbContext _dbContext;

        public RequestClientRepository(HalloDocDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Region> GetAllRegions()
        {
            return _dbContext.Regions.ToList();
        }

        public List<RequestClient> GetAllRequestClients()
        {
            return _dbContext.RequestClients.Include(a => a.Request) .ToList();
        }

        public List<RequestClient> GetRequestClientsBasedOnFilter(Func<RequestClient, bool> predicate)
        {
            return _dbContext.RequestClients.Include(a => a.Request).ThenInclude(a => a.RequestNotes).Include(a => a.Physician)
                                            .Include(a => a.Request.RequestStatusLogs).Where(predicate).ToList();
        }

        public List<BlockRequest> GetRequestClientsAndBlockRequestBasedOnFilter(Func<BlockRequest, bool> predicate)
        {
            return _dbContext.BlockRequests.Include(a => a.Request.RequestClients).Where(predicate).ToList();
        }

        public int CountRequestClientsAndBlockRequestBasedOnFilter(Func<BlockRequest, bool> predicate)
        {
            return _dbContext.BlockRequests.Include(a => a.Request.RequestClients).Count(predicate);
        }

        public List<RequestClient> GetRequestClientByStatus(Func<RequestClient, bool> predicate, int skip)
        {
            return _dbContext.RequestClients.Include(a => a.Request).ThenInclude(a => a.Encounters).Include(a => a.Physician).Where(predicate)
                                          .OrderByDescending(a => a.RequestClientId).Skip(skip).Take(10).ToList();
        }

        public int CountRequestClientByStatusAndFilter(Func<RequestClient, bool> predicate)
        {
            return _dbContext.RequestClients.Include(a => a.Request).Include(a => a.Physician).Count(predicate);
        }

        public int CountRequestClientByStatusForAdmin(List<int> status)
        {
            return _dbContext.RequestClients.Count(a => status.Contains(a.Status));
        }

        public int CountRequestClientByStatusForPhysician(List<int> status, int aspNetUserId)
        {
            Func<RequestClient, bool> predicate = a => 
            status.Contains(a.Status) 
            && (!status.Contains(1) || a.Physician != null) 
            && a.Physician != null
            && a.Physician.AspNetUserId == aspNetUserId;
            return _dbContext.RequestClients.Include(a => a.Physician).Count(predicate);
        }

        public List<RequestClient> GetAllRequestClientForUser(int userId)
        {
            return _dbContext.RequestClients.Include(a => a.Request.RequestWiseFiles.Where(a => a.IsDeleted != new BitArray(1, true)))
                                                   .Where(a => a.Request.UserId == userId).OrderByDescending(a => a.RequestClientId).ToList();
        }

        public async Task<bool> AddRequestClient(RequestClient requestClient)
        {
            _dbContext.RequestClients.Add(requestClient);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public RequestClient GetRequestClientByRequestId(int requestId)
        {
            return _dbContext.RequestClients.FirstOrDefault(a => a.RequestId == requestId);
        }

        public RequestClient GetRequestClientAndRequestByRequestId(int requestId)
        {
            return _dbContext.RequestClients.Include(a => a.Request).FirstOrDefault(a => a.RequestId == requestId);
        }

        public async Task<bool> UpdateRequestClient(RequestClient requestClient)
        {
            _dbContext.RequestClients.Update(requestClient);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public List<CaseTag> GetAllReason()
        {
            return _dbContext.CaseTags.ToList();
        }

        public List<RequestClient> GetAllRequestClientsByUserId(int userId, int skip)
        {
            return _dbContext.RequestClients.Include(a => a.Request).ThenInclude(a => a.RequestWiseFiles.Where(a => a.IsDeleted != new BitArray(1, true)))
                                     .Include(a => a.Request.Encounters).Include(a => a.Physician).Where(a => a.Request.UserId == userId)
                                                    .OrderByDescending(a => a.RequestClientId).Skip(skip).Take(5).ToList();
        }

        public int CountRequestClientsByUserId(int userId)
        {
            return _dbContext.RequestClients.Include(a => a.Request).Count(a => a.Request.UserId == userId);
        }

        public async Task<bool> DeleteBlockRequest(int requestId)
        {
            _dbContext.Remove(_dbContext.BlockRequests.First(a => a.RequestId == requestId));
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}