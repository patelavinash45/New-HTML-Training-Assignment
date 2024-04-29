using Microsoft.EntityFrameworkCore;
using Repositories.DataContext;
using Repositories.DataModels;
using Repositories.Interface;

namespace Repositories.Implementation
{
    public class AspRepository : IAspRepository
    {
        private readonly HalloDocDbContext _dbContext;

        public AspRepository(HalloDocDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int CheckUser(String email)
        {
            AspNetUser aspNetUser = _dbContext.AspNetUsers.FirstOrDefault(a => a.Email.Trim() == email.Trim());
            return aspNetUser?.Id ?? 0;
        }

        public async Task<int> AddUser(AspNetUser aspNetUser)
        {
            _dbContext.AspNetUsers.Add(aspNetUser);
            await _dbContext.SaveChangesAsync();
            return aspNetUser?.Id ?? 0;
        }

        public bool CheckToken(String token, int aspNetUserId)
        {
            AspNetUser aspNetUser = _dbContext.AspNetUsers.FirstOrDefault(a => a.Id == aspNetUserId && a.ResetPasswordToken == token);
            return aspNetUser != null;
        }

        public async Task<bool> SetToken(String token, int aspNetUserId)
        {
            AspNetUser aspNetUser = _dbContext.AspNetUsers.FirstOrDefault(a => a.Id == aspNetUserId);
            if(aspNetUser!=null)
            {
                aspNetUser.ResetPasswordToken = token;
                _dbContext.AspNetUsers.Update(aspNetUser);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            return false;
        }

        public AspNetUser GetUser(int aspNetUserId)
        {
            return _dbContext.AspNetUsers.FirstOrDefault(a => a.Id == aspNetUserId);
        }

        public AspNetUser GetUserFromEmail(String email)
        {
            return _dbContext.AspNetUsers.FirstOrDefault(a => a.Email == email);
        }

        public async Task<bool> ChangePassword(AspNetUser aspNetUser)
        {
            _dbContext.Update(aspNetUser); 
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public int CheckUserRole(string role)
        {
            AspNetRole aspNetRole = _dbContext.AspNetRoles.FirstOrDefault(a => a.Name.Trim() == role);
            return aspNetRole?.Id ?? 0;
        }

        public async Task<int> AddUserRole(AspNetRole aspNetRole)
        {
            _dbContext.AspNetRoles.Add(aspNetRole);
            await _dbContext.SaveChangesAsync();
            return aspNetRole?.Id ?? 0;
        }

        public async Task<bool> AddAspNetUserRole(AspNetUserRole aspNetUserRole)
        {
            _dbContext.AspNetUserRoles.Add(aspNetUserRole);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public AspNetUserRole ValidateAspNetUserRole(String email, String password)
        {
            return _dbContext.AspNetUserRoles.Include(a => a.User).Include(a => a.Role)
                       .FirstOrDefault(a => a.User.Email == email && a.User.PasswordHash == password);
        }
    }
}

