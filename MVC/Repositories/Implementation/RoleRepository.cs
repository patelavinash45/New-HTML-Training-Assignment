using Microsoft.EntityFrameworkCore;
using Repositories.DataContext;
using Repositories.DataModels;
using Repositories.Interfaces;
using System.Collections;
using System.Data;

namespace Repositories.Implementation
{
    public class RoleRepository : IRoleRepository
    {
        private readonly HalloDocDbContext _dbContext;

        public RoleRepository(HalloDocDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Role> GetAllRoles()
        {
            return _dbContext.Roles.Include(a => a.AccountTypeNavigation).Where(a => a.IsDeleted != new BitArray(1, true))
                                                               .OrderByDescending(a => a.RoleId).ToList();
        }

        public List<Role> GetRolesByUserType(int type)
        {
            return _dbContext.Roles.Include(a => a.AccountTypeNavigation).Where(a => a.AccountType == type).ToList();
        }

        public Role GetRoleByRoleId(int roleId)
        {
            return _dbContext.Roles.FirstOrDefault(a => a.RoleId == roleId);
        }

        public Role GetRoleByName(string roleName)
        {
            return _dbContext.Roles.FirstOrDefault(a => a.Name.ToLower() == roleName.ToLower());
        }

        public List<Menu> GetAllMenus()
        {
            return _dbContext.Menus.ToList();
        }

        public List<Menu> GetAllMenusByRole(int roleId)
        {
            return _dbContext.Menus.Where(a => a.AccountType == roleId).ToList();
        }

        public List<RoleMenu> GetAllRoleMenusByRole(int roleId)
        {
            return _dbContext.RoleMenus.Where(a => a.RoleId == roleId).ToList();
        }

        public async Task<int> AddRole(Role role)
        {
            _dbContext.Roles.Add(role);
            await _dbContext.SaveChangesAsync();
            return role?.RoleId ?? 0;
        }

        public async Task<bool> AddRoleMenus(List<RoleMenu> roleMenus)
        {
            _dbContext.RoleMenus.AddRange(roleMenus);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateRole(Role role)
        {
            _dbContext.Roles.Update(role);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteRoleMenus(List<RoleMenu> roleMenus)
        {
            _dbContext.RoleMenus.RemoveRange(roleMenus);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public Admin GetRoleWithRoleMenusAndAdmin(int aspNetUserId, int menuId)
        {
            return _dbContext.Admins.Include(a => a.Role).ThenInclude(a => a.RoleMenus.Where(x => x.MenuId == menuId))
                                                 .FirstOrDefault(a => a.AspNetUserId == aspNetUserId);
        }
        
        public Physician GetRoleWithRoleMenusAndPhysician(int aspNetUserId, int menuId)
        {
            return _dbContext.Physicians.Include(a => a.Role).ThenInclude(a => a.RoleMenus.Where(x => x.MenuId == menuId))
                                                 .FirstOrDefault(a => a.AspNetUserId == aspNetUserId);
        }
    }
}
