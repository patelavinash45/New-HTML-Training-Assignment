﻿using Microsoft.AspNetCore.Http;
using Repositories.DataModels;
using Repositories.Interface;
using Repositories.Interfaces;
using Services.Interfaces.AuthServices;
using Services.ViewModels;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services.Implementation.AuthServices
{
    public class LoginService : ILoginService
    {
        private readonly IAspRepository _aspRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogsService _logsService;

        public LoginService(IAspRepository aspRepository, IUserRepository userRepository, IJwtService jwtService, IRoleRepository roleRepository, 
                                                                 ILogsService logsService)
        {
            _aspRepository = aspRepository;
            _userRepository = userRepository;
            _jwtService = jwtService;
            _roleRepository = roleRepository;
            _logsService = logsService;
        }

        public UserDataModel auth(Login model,List<int> userType)
        {
            AspNetUserRole aspNetUserRole = _aspRepository.validateAspNetUserRole(email: model.Email, password: genrateHash(model.PasswordHash));
            if (aspNetUserRole != null)
            {
                if(userType.Contains(aspNetUserRole.RoleId))
                {
                    switch (aspNetUserRole.RoleId)
                    {
                        case 1: User user = _userRepository.getUser(aspNetUserRole.UserId);
                                return new UserDataModel()
                                {
                                    UserTypeId = 1,
                                    IsValid = true,
                                    AspNetUserId = aspNetUserRole.UserId,
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    UserType = aspNetUserRole.Role.Name,
                                    Message = "Successfully Login",
                                };
                        case 2: Admin admin = _userRepository.getAdmionByAspNetUserId(aspNetUserRole.UserId);
                                return new UserDataModel()
                                {
                                    UserTypeId = 2,
                                    IsValid = true,
                                    AspNetUserId = aspNetUserRole.UserId,
                                    FirstName = admin.FirstName,
                                    LastName = admin.LastName,
                                    UserType = aspNetUserRole.Role.Name,
                                    Message = "Successfully Login",
                                };
                        default: Physician physician = _userRepository.getPhysicianByAspNetUserId(aspNetUserRole.UserId);
                                 return new UserDataModel()
                                 {
                                     UserTypeId = 3,
                                     IsValid = true,
                                     AspNetUserId = aspNetUserRole.UserId,
                                     FirstName = physician.FirstName,
                                     LastName = physician.LastName,
                                     UserType = aspNetUserRole.Role.Name,
                                     Message = "Successfully Login",
                                 };
                    }
                }
                else
                {
                    return new UserDataModel()
                    {
                        Message = "Access Denied !!",
                    };
                }
            }
            return new UserDataModel()
            {
                Message = "Invalid credentials !!",
            };
        }

        public string isTokenValid(HttpContext httpContext,List<int> userType)
        {
            String token = httpContext.Request.Cookies["jwtToken"];
            if (token != null)
            {
                JwtSecurityToken jwtToken = new JwtSecurityToken();
                if (_jwtService.validateToken(token, out jwtToken))
                {
                    var jwtUserType = jwtToken.Claims.FirstOrDefault(a => a.Type == "userTypeId");
                    if (userType.Contains(int.Parse(jwtUserType.Value)))
                    {
                        var jwtRole = jwtToken.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role);
                        var jwtId = jwtToken.Claims.FirstOrDefault(a => a.Type == "aspNetUserId");
                        var jwtFirstName = jwtToken.Claims.FirstOrDefault(a => a.Type == "firstName");
                        var jwtLastName = jwtToken.Claims.FirstOrDefault(a => a.Type == "lastName");
                        httpContext.Session.SetString("role", jwtRole.Value);
                        httpContext.Session.SetString("firstName", jwtFirstName.Value);
                        httpContext.Session.SetString("lastName", jwtLastName.Value);
                        httpContext.Session.SetInt32("aspNetUserId", int.Parse(jwtId.Value));
                        return jwtRole.Value;
                    }
                }
            }
            return null;
        }

        public async Task<bool> resetPasswordLinkSend(string email,HttpContext httpContext)
        {
            try
            {
                var request = httpContext.Request;
                AspNetUser aspNetUser = _aspRepository.getUserFromEmail(email);
                if(aspNetUser.Id != 0) 
                {
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim("aspNetUserId", aspNetUser.Id.ToString()),
                    };
                    String token = _jwtService.genrateJwtTokenForSendMail(claims, DateTime.Now.AddDays(1));
                    await _aspRepository.setToken(token: token, aspNetUserId: aspNetUser.Id);
                    string link = $"{request.Scheme}://{request.Host}/Patient/Newpassword?token={token}";
                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress("tatva.dotnet.avinashpatel@outlook.com"),
                        Subject = "Reset Password Link",
                        IsBodyHtml = true,
                    };
                    string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/EmailTemplate/resetPasswordEmail.cshtml");
                    string body = File.ReadAllText(templatePath).Replace("EmailLink", link);
                    mailMessage.Body = body;
                    mailMessage.To.Add("tatva.dotnet.avinashpatel@outlook.com");
                    SmtpClient smtpClient = new SmtpClient("smtp.office365.com")
                    {
                        UseDefaultCredentials = false,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        EnableSsl = true,
                        Port = 587,
                        Credentials = new NetworkCredential(userName: "tatva.dotnet.avinashpatel@outlook.com", password: "Avinash@6351"),
                    };
                    smtpClient.SendMailAsync(mailMessage);
                    EmailLog emailLog = new EmailLog()
                    {
                        Name = aspNetUser.UserName,
                        SubjectName = "Reset Password Link Send",
                        EmailId = email,
                        CreateDate = DateTime.Now,
                        SentDate = DateTime.Now,
                        IsEmailSent = new BitArray(1, true),
                        RoleId = null,
                    };
                    return await _logsService.addEmailLog(emailLog);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        public SetNewPassword validatePasswordLink(string token)
        {
            SetNewPassword setNewPassword = new()
            {
                IsValidLink = false,
            };
            try
            {
                JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(token);
                if (_jwtService.validateToken(token, out jwtSecurityToken))
                {
                    int aspNetUserId = int.Parse(jwtSecurityToken.Claims.FirstOrDefault(a => a.Type == "aspNetUserId").Value);
                    if(_aspRepository.checkToken(token: token, aspNetUserId: aspNetUserId))
                    {
                        setNewPassword.IsValidLink = true;
                        setNewPassword.AspNetUserId = aspNetUserId;
                        return setNewPassword;
                    }
                    else
                    {
                        setNewPassword.ErrorMessage = "Link is Expired";
                    }
                }
                else
                {
                    setNewPassword.ErrorMessage = "Link is Expired";
                }
            }
            catch(Exception ex)
            {
                setNewPassword.ErrorMessage = "Link is Not Vlaid";
            }
            return setNewPassword;
        }

        public Task<bool> changePassword(int aspNetUserId, String password)
        {
            AspNetUser aspNetUser = _aspRepository.getUser(aspNetUserId);
            aspNetUser.PasswordHash = genrateHash(password);
            aspNetUser.ResetPasswordToken = null;
            return _aspRepository.changePassword(aspNetUser);
        }

        public bool validateAccess(int aspNetUserId, int menuId, bool isAdmin)
        {
            if(isAdmin)
            {
                Admin admin = _roleRepository.getRoleWithRoleMenusAndAdmin(aspNetUserId, menuId);
                if (admin.Role.RoleMenus.Count == 1)
                {
                    return true;
                }
            }
            else
            {
                Physician physician = _roleRepository.getRoleWithRoleMenusAndPhysician(aspNetUserId, menuId);
                if (physician.Role.RoleMenus.Count == 1)
                {
                    return true;
                }
            }
            return false;
        }

        private string genrateHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(password.Trim()));
                return BitConverter.ToString(hashPassword).Replace("-", "").ToLower();
            }
        }
    }
}

