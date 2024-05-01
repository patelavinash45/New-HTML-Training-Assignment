using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IRoleRepository
    {
        List<Role> GetAllRoles();

        List<Role> GetRolesByUserType(int type);

        Role GetRoleByRoleId(int roleId);

        Role GetRoleByName(string roleName);

        List<Menu> GetAllMenus();

        List<Menu> GetAllMenusByRole(int roleId);

        List<RoleMenu> GetAllRoleMenusByRole(int roleId);

        Task<int> AddRole(Role role);

        Task<bool> AddRoleMenus(List<RoleMenu> roleMenus);

        Task<bool> UpdateRole(Role role);

        Task<bool> DeleteRoleMenus(List<RoleMenu> roleMenus);

        Admin GetRoleWithRoleMenusAndAdmin(int aspNetUserId, int menuId);

        Physician GetRoleWithRoleMenusAndPhysician(int aspNetUserId, int menuId);
    }
}
