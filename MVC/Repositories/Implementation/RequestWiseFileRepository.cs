using Repositories.DataContext;
using Repositories.DataModels;
using Repositories.Interfaces;
using System.Collections;

namespace Repositories.Implementation
{
    public class RequestWiseFileRepository : IRequestWiseFileRepository
    {
        private readonly HalloDocDbContext _dbContext;

        public RequestWiseFileRepository(HalloDocDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddFile(RequestWiseFile requestWiseFile)
        {
            _dbContext.RequestWiseFiles.Add(requestWiseFile);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public List<RequestWiseFile> GetFilesByrequestId(int requestId)
        {
            return _dbContext.RequestWiseFiles.Where(a => a.RequestId == requestId && a.IsDeleted != new BitArray(1, true)).ToList();
        }

        public List<RequestWiseFile> GetRequestWiseFilesByIds(List<int> ids)
        {
            return _dbContext.RequestWiseFiles.Where(a => ids.Contains(a.RequestWiseFileId)).ToList();
        }

        public RequestWiseFile GetFilesByrequestWiseFileId(int requestWiseFileId)
        {
            return _dbContext.RequestWiseFiles.FirstOrDefault(a => a.RequestWiseFileId == requestWiseFileId);
        }

        public async Task<bool> UpdateRequestWiseFiles(List<RequestWiseFile> requestWiseFiles)
        {
            _dbContext.RequestWiseFiles.UpdateRange(requestWiseFiles);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
