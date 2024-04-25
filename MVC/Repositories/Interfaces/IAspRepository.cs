using Repositories.DataModels;

namespace Repositories.Interface
{
    public interface IAspRepository
    {
        int CheckUser(String email);

        AspNetUser GetUser(int aspNetUserId);

        AspNetUser GetUserFromEmail(String email);

        Task<int> AddUser(AspNetUser aspNetUser);

        Task<bool> SetToken(String token, int aspNetUserId);

        bool CheckToken(String token, int aspNetUserId);

        Task<bool> ChangePassword(AspNetUser aspNetUser);

        int CheckUserRole(string role);

        Task<int> AddUserRole(AspNetRole aspNetRole);

        Task<bool> AddAspNetUserRole(AspNetUserRole aspNetUserRole);

        AspNetUserRole ValidateAspNetUserRole(string email, string password);
    }
}
