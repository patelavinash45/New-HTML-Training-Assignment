using Repositories.DataModels;
using Repositories.Interface;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Interfaces.AdminServices;
using Services.ViewModels.Admin;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Services.Implementation.AdminServices
{
    public class AccessService : IAccessService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRequestClientRepository _requestClientRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAspRepository _aspRepository;
        private readonly IFileService _fileService;

        public AccessService(IRoleRepository roleRepository, IRequestClientRepository requestClientRepository, IAspRepository aspRepository,
                                         IUserRepository userRepository, IFileService fileService)
        {
            _roleRepository = roleRepository;
            _requestClientRepository = requestClientRepository;
            _aspRepository = aspRepository;
            _userRepository = userRepository;
            _fileService = fileService;
        }

        public Access GetAccessData()
        {
            return new Access()
            {
                RolesData = _roleRepository.GetAllRoles().Select(role => new AccessTable()
                {
                    Name = role.Name,
                    AccountType = role.AccountTypeNavigation.Name,
                    RoleId = role.RoleId,
                }).ToList(),
            };
        }

        public CreateRole GetCreateRole()
        {
            return new CreateRole()
            {
                RolesCheckBox = new RolesCheckBox()
                {
                    SelectedMenusForLoadData = new List<int>(),
                    Menus = _roleRepository.GetAllMenus().ToDictionary(menu => menu.MenuId, menu => menu.Name),
                },
            };
        }

        public RolesCheckBox GetMenusByRole(int roleId)
        {
            return new RolesCheckBox()
            {
                SelectedMenusForLoadData = new List<int>(),
                Menus = _roleRepository.GetAllMenusByRole(roleId).ToDictionary(menu => menu.MenuId, menu => menu.Name),
            };
        }

        public async Task<bool> CreateRole(CreateRole model, int aspNetUserId)
        {
            Role role = new Role()
            {
                Name = model.RoleName,
                AccountType = model.SlectedAccountType,
                CreatedDate = DateTime.Now,
                CreatedBy = aspNetUserId.ToString(),
            };
            int roleId = await _roleRepository.AddRole(role);
            if (roleId > 0 && model.SelectedMenus.Count > 0)
            {
                await _roleRepository.AddRoleMenus(
                        model.SelectedMenus.Select(menuId =>
                        new RoleMenu()
                        {
                            RoleId = roleId,
                            MenuId = menuId,
                        }).ToList()
                );
                return true;
            }
            else if (roleId > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<String> DeleteRole(int roleId, int aspNetUserId)
        {
            if (roleId != 14 && roleId != 15)
            {
                if (await _roleRepository.DeleteRoleMenus(_roleRepository.GetAllRoleMenusByRole(roleId)))
                {
                    Role role = _roleRepository.GetRoleByRoleId(roleId);
                    role.IsDeleted = new BitArray(1, true);
                    role.ModifiedDate = DateTime.Now;
                    role.ModifiedBy = aspNetUserId.ToString();
                    return await _roleRepository.UpdateRole(role) ? "" : "Failed !!";
                }
                return "Failed !!";
            }
            return "This Role can not be Delete";
        }

        public AdminCreaateAndProfile GetAdminCreaateAndProfile()
        {
            return new AdminCreaateAndProfile()
            {
                Regions = _requestClientRepository.GetAllRegions().ToDictionary(region => region.RegionId, region => region.Name),
                Roles = _roleRepository.GetRolesByUserType(2).ToDictionary(role => role.RoleId, role => role.Name),
            };
        }

        public async Task<String> CreateAdmin(AdminCreaateAndProfile model)
        {
            if (_aspRepository.CheckUser(model.Email) == 0)
            {
                int aspNetRoleId = _aspRepository.CheckUserRole(role: "Admin");
                if (aspNetRoleId == 0)
                {
                    AspNetRole aspNetRole = new()
                    {
                        Name = "Admin",
                    };
                    aspNetRoleId = await _aspRepository.AddUserRole(aspNetRole);
                }
                AspNetUser aspNetUser = new()
                {
                    UserName = model.FirstName,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    PasswordHash = GenrateHash(model.Password),
                    CreatedDate = DateTime.Now,
                };
                int aspNetUserId = await _aspRepository.AddUser(aspNetUser);
                AspNetUserRole aspNetUserRole = new()
                {
                    UserId = aspNetUserId,
                    RoleId = aspNetRoleId,
                };
                await _aspRepository.AddAspNetUserRole(aspNetUserRole);
                Admin admin = new Admin()
                {
                    AspNetUserId = aspNetUserId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    City = model.City,
                    RegionId = int.Parse(model.SelectedRegion),
                    Zip = model.ZipCode,
                    AltPhone = model.Phone,
                    Status = model.Status,
                    RoleId = model.SelectedRole,
                };
                if (await _userRepository.AddAdmin(admin))
                {
                    List<AdminRegion> adminRegions = new List<AdminRegion>();
                    foreach (String regionId in model.SelectedRegions)
                    {
                        adminRegions.Add(new AdminRegion()
                        {
                            AdminId = admin.AdminId,
                            RegionId = int.Parse(regionId),
                        });
                    }
                    _fileService.SendNewAccountMail(model.Email, model.Password);
                    return await _userRepository.AddAdminRgions(adminRegions) ? "" : "Failed !!";
                }
                return "Failed !!";
            }
            return "Email Already Exits";
        }

        public CreateRole GetEditRole(int roleId)
        {
            Role role = _roleRepository.GetRoleByRoleId(roleId);
            return new CreateRole()
            {
                IsUpdate = true,
                RoleName = role.Name,
                SlectedAccountType = role.AccountType,
                RolesCheckBox = new RolesCheckBox()
                {
                    SelectedMenusForLoadData = _roleRepository.GetAllRoleMenusByRole(roleId).Select(roleMenu => roleMenu.MenuId).ToList(),
                    Menus = _roleRepository.GetAllMenusByRole(role.AccountType).ToDictionary(menu => menu.MenuId, menu => menu.Name),
                },
            };
        }

        public async Task<bool> EditRole(CreateRole model, int roleId, int aspNetUserId)
        {
            List<RoleMenu> roleMenus = _roleRepository.GetAllRoleMenusByRole(roleId);
            List<RoleMenu> roleMenusForDelete = new List<RoleMenu>();
            List<RoleMenu> roleMenusCreateNew = new List<RoleMenu>();
            foreach (RoleMenu roleMenu in roleMenus)
            {
                if (!model.SelectedMenus.Contains(roleMenu.MenuId))
                {
                    roleMenusForDelete.Add(roleMenu);
                }
            }
            foreach (int menuId in model.SelectedMenus)
            {
                if (!roleMenus.Any(a => a.MenuId == menuId))
                {
                    RoleMenu roleMenu = new RoleMenu()
                    {
                        RoleId = roleId,
                        MenuId = menuId,
                    };
                    roleMenusCreateNew.Add(roleMenu);
                }
            }
            if (roleMenusForDelete.Count > 0)
            {
                await _roleRepository.DeleteRoleMenus(roleMenusForDelete);
            }
            if (roleMenusCreateNew.Count > 0)
            {
                await _roleRepository.AddRoleMenus(roleMenusCreateNew);
            }
            Role role = _roleRepository.GetRoleByRoleId(roleId);
            role.Name = model.RoleName;
            role.AccountType = model.SlectedAccountType;
            role.ModifiedDate = DateTime.Now;
            role.ModifiedBy = aspNetUserId.ToString();
            return await _roleRepository.UpdateRole(role);
        }

        private String GenrateHash(String password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(password.Trim()));
                return BitConverter.ToString(hashPassword).Replace("-", "").ToLower();
            }
        }
    }
}
