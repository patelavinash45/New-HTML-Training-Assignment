using Microsoft.AspNetCore.Http;

namespace Services.Interfaces
{
    public interface IFileService
    {
        Task<bool> AddFile(IFormFile file, int requestId, string firstName, string lastName);

        void SendNewAccountMail(string email, string password);
    }
}
