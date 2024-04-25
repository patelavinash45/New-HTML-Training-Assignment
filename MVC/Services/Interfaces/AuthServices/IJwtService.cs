using Services.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Services.Interfaces.AuthServices
{
    public interface IJwtService
    {
        string GenrateJwtToken(UserDataModel user);

        string GenrateJwtTokenForSendMail(List<Claim> claims, DateTime expries);

        bool ValidateToken(String token, out JwtSecurityToken jwtToken);
    }
}
