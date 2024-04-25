using Microsoft.AspNetCore.Http;
using Services.ViewModels;

namespace Services.Interfaces.AuthServices
{
    public interface ILoginService
    {
        UserDataModel Auth(Login model,List<int> userType);

        String IsTokenValid(HttpContext httpContext, List<int> userType);

        Task<bool> ResetPasswordLinkSend(string email, HttpContext httpContext);

        SetNewPassword ValidatePasswordLink(string token);

        Task<bool> ChangePassword(int aspNetUserId, String password);

        bool ValidateAccess(int aspNetUserId, int menuId, bool isAdmin);

    }
}
