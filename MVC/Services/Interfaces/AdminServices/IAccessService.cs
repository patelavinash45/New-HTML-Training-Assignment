﻿using Services.ViewModels.Admin;

namespace Services.Interfaces.AdminServices
{
    public interface IAccessService
    {
        Access GetAccessData();

        CreateRole GetCreateRole();

        RolesCheckBox GetMenusByRole(int roleId);

        Task<bool> CreateRole(CreateRole model, int aspNetUserId);

        Task<String> DeleteRole(int roleId, int aspNetUserId);

        AdminCreateAndProfile GetAdminCreateAndProfile();

        Task<String> CreateAdmin(AdminCreateAndProfile model);

        CreateRole GetEditRole(int roleId);

        Task<bool> EditRole(CreateRole model, int roleId, int aspNetUserId);
    }
}
