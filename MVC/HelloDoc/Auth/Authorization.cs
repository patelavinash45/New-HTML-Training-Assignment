using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Services.Interfaces.AuthServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HelloDoc.Auth
{
    public class Authorization : Attribute, IAuthorizationFilter
    {
        private List<string> _roles;
        private int _menuId;

        public Authorization(params string[] roles)
        {
            _roles = new List<string>(roles);
            _menuId = 0;
        }

        public Authorization(int menuId, params string[] roles)
        {
            _roles = new List<string>(roles);
            _menuId = menuId;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            IJwtService jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();
            if (jwtService != null)
            {
                string token = context.HttpContext.Request.Cookies["jwtToken"];
                JwtSecurityToken jwtToken = new JwtSecurityToken();
                if (token == null)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        Controller = _roles.Contains("Patient") ? "Patient" : "Admin",
                        action = "LoginPage",
                    }));
                }
                else if (token != null && !jwtService.ValidateToken(token, out jwtToken))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        Controller = "Patient",
                        action = "PatientSite",
                    }));
                }
                else
                {
                    string jwtRole = jwtToken.Claims.First(a => a.Type == ClaimTypes.Role).Value;
                    if (!_roles.Contains(jwtRole))
                    {
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                        {
                            Controller = "Patient",
                            action = "AccessDenied",
                        }));
                    }
                    else
                    {
                        int jwtId = int.Parse(jwtToken.Claims.First(a => a.Type == "aspNetUserId").Value);
                        if (context.HttpContext.Session.GetInt32("aspNetUserId") == null)
                        {
                            string jwtFirstName = jwtToken.Claims.First(a => a.Type == "firstName").Value;
                            string jwtLastName = jwtToken.Claims.First(a => a.Type == "lastName").Value;
                            context.HttpContext.Session.SetString("role", jwtRole);
                            context.HttpContext.Session.SetString("firstName", jwtFirstName);
                            context.HttpContext.Session.SetString("lastName", jwtLastName);
                            context.HttpContext.Session.SetInt32("aspNetUserId", jwtId);
                        }
                        if (_menuId > 0)
                        {
                            ILoginService loginService = context.HttpContext.RequestServices.GetService<ILoginService>();
                            if (loginService != null)
                            {
                                bool isAdmin = jwtRole == "Admin";
                                if (!loginService.ValidateAccess(jwtId, _menuId, isAdmin))
                                {
                                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                                    {
                                        Controller = jwtRole,
                                        action = "AccessDenied",
                                    }));
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
