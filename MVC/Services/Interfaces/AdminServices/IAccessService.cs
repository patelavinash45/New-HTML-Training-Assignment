using Services.ViewModels.Admin;

namespace Services.Interfaces.AdminServices
{
    public interface IAccessService
    {
        Access getAccessData();

        CreateRole getCreateRole();

        RolesCheckBox getMenusByRole(int roleId);

        Task<bool> createRole(CreateRole model, int aspNetUserId);

        Task<String> delete(int roleId, int aspNetUserId);

        AdminCreaateAndProfile GetAdminCreaateAndProfile();

        Task<String> createAdmin(AdminCreaateAndProfile model);

        CreateRole getEditRole(int roleId);

        Task<bool> editRole(CreateRole model,int roleId,int aspNetUserId);
    }
}
