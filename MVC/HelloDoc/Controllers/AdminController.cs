using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using HelloDoc.Auth;
using Microsoft.AspNetCore.Mvc;
using Repositories.DataModels;
using Services.Interfaces;
using Services.Interfaces.AdminServices;
using Services.Interfaces.AuthServices;
using Services.ViewModels;
using Services.ViewModels.Admin;
using System.Data;

namespace HelloDoc.Controllers
{
    public class AdminController : Controller
    {
        private readonly INotyfService _notyfService;
        private readonly ILoginService _loginService;
        private readonly IAdminDashboardService _adminDashboardService;
        private readonly IViewNotesService _viewNotesService;
        private readonly IViewDocumentsServices _viewDocumentsServices;
        private readonly IJwtService _jwtService;
        private readonly ISendOrderService _sendOrderService;
        private readonly ICloseCaseService _closeCaseService;
        private readonly IViewProfileService _viewProfileService;
        private readonly IProvidersService _providersService;
        private readonly IAccessService _accessService;
        private readonly IPartnersService _partnersService;
        private readonly IRecordService _recordService;

        public AdminController(INotyfService notyfService, IAdminDashboardService adminDashboardService,
                                IViewNotesService viewNotesService, ILoginService loginService, IViewDocumentsServices viewDocumentsServices,
                                IJwtService jwtService, ISendOrderService sendOrderService, ICloseCaseService closeCaseService,
                                IViewProfileService viewProfileService, IPartnersService partnersService,
                                IProvidersService providersService, IAccessService accessService, IRecordService recordService)
        {
            _notyfService = notyfService;
            _loginService = loginService;
            _adminDashboardService = adminDashboardService;
            _viewNotesService = viewNotesService;
            _viewDocumentsServices = viewDocumentsServices;
            _jwtService = jwtService;
            _partnersService = partnersService;
            _sendOrderService = sendOrderService;
            _closeCaseService = closeCaseService;
            _viewProfileService = viewProfileService;
            _providersService = providersService;
            _accessService = accessService;
            _recordService = recordService;
        }

        public IActionResult LoginPage()
        {
            string role = _loginService.IsTokenValid(HttpContext, new List<int> { 2, 3 });
            if (role != null)
            {
                return RedirectToAction("Dashboard", role);
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("aspNetUserId");
            HttpContext.Session.Remove("role");
            Response.Cookies.Delete("jwtToken");
            _notyfService.Success("Successfully Logout");
            return RedirectToAction("LoginPage", "Admin");
        }

        public IActionResult Chat()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorization(6, "Admin")]
        public IActionResult Dashboard()
        {
            return View(_adminDashboardService.GetAllRequests());
        }

        [Authorization("Admin", "Physician")]
        [HttpGet("/Case/ConcludeCare")]
        public IActionResult ConcludeCare()
        {
            int requestId = HttpContext.Session.GetInt32("requestId").Value;
            return View(_adminDashboardService.GetConcludeCare(requestId));
        }

        [Authorization("Admin", "Physician")]
        [HttpGet("/Encounter/EncounterForm")]
        public IActionResult EncounterForm()
        {
            int requestId = HttpContext.Session.GetInt32("requestId").Value;
            bool isAdmin = HttpContext.Session.GetString("role") == "Admin";
            return View(_adminDashboardService.GetEncounterDetails(requestId, isAdmin));
        }

        [Authorization("Admin", "Physician")]
        [HttpGet("/Case/ViewCase")]
        public IActionResult ViewCase()
        {
            int requestId = HttpContext.Session.GetInt32("requestId").Value;
            bool isAdmin = HttpContext.Session.GetString("role") == "Admin";
            return View(_adminDashboardService.GetRequestDetails(requestId, isAdmin));
        }

        [Authorization("Admin", "Physician")]
        [HttpGet("/Case/ViewNotes")]
        public IActionResult ViewNotes()
        {
            int requestId = HttpContext.Session.GetInt32("requestId").Value;
            return View(_viewNotesService.GetNotes(requestId));
        }

        [Authorization("Admin", "Physician")]
        [HttpGet("/Request/CreateRequest")]
        public IActionResult CreateRequest()
        {
            return View(null);
        }

        [Authorization("Admin")]
        public IActionResult CreateProvider()
        {
            return View(_providersService.GetCreateProvider());
        }

        [Authorization(4, "Admin")]
        public IActionResult CreateAdmin()
        {
            return View(_accessService.GetAdminCreateAndProfile());
        }

        [Authorization(8, "Admin")]
        public IActionResult Providers()
        {
            return View(_providersService.GetProviders(regionId: 0));
        }

        [Authorization(16, "Admin")]
        public IActionResult Partners()
        {
            return View(_partnersService.GetPartnersData());
        }

        [Authorization("Admin")]
        public IActionResult BusinessProfile()
        {
            return View(_partnersService.AddBusiness(isUpdate: false, venderId: 0));
        }

        [Authorization(7, "Admin")]
        public IActionResult Access()
        {
            return View(_accessService.GetAccessData());
        }

        [Authorization("Admin")]
        public IActionResult ProviderOnCall()
        {
            return View(_providersService.GetProviderOnCall(regionId: 0));
        }

        [Authorization("Admin")]
        public IActionResult CreateRole()
        {
            return View(_accessService.GetCreateRole());
        }

        [Authorization("Admin")]
        public IActionResult PayRate()
        {
            int physicianId = HttpContext.Session.GetInt32("physicianId").Value;
            return View(_providersService.GetPayRate(physicianId));
        }

        [Authorization(20, "Admin")]
        public IActionResult Records()
        {
            return View(_recordService.GetRecords(new Records()));
        }

        [Authorization(13, "Admin")]
        public IActionResult EmailLogs()
        {
            return View(_recordService.GetEmailLog(new EmailSmsLogs()));
        }

        [Authorization(17, "Admin")]
        public IActionResult SMSLogs()
        {
            return View(_recordService.GetSMSLog(new EmailSmsLogs()));
        }

        [Authorization("Admin")]
        public IActionResult PatientHistory()
        {
            return View(_recordService.GetPatientHistory(new PatientHistory(), pageNo: 1));
        }

        [Authorization("Admin")]
        public IActionResult PatientRecord()
        {
            int userId = HttpContext.Session.GetInt32("userId").Value;
            return View(_recordService.GetPatientRecord(userId, pageNo: 1));
        }

        [Authorization(2, "Admin")]
        public IActionResult BlockHistory()
        {
            return View(_recordService.GetBlockHistory(new BlockHistory(), pageNo: 1));
        }

        [Authorization(5, "Admin")]
        public IActionResult ViewProfile()
        {
            int aspNetUseId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            return View(_viewProfileService.GetAdminViewProfile(aspNetUseId));
        }

        [Authorization(3, "Admin")]
        public IActionResult ProviderScheduling()
        {
            return View(_providersService.GetProviderSchedulingData());
        }

        [Authorization("Admin")]
        public IActionResult RequestedShift()
        {
            return View(_providersService.GetRequestedShift());
        }

        [Authorization("Admin")]
        public IActionResult CloseCase()
        {
            int requestId = HttpContext.Session.GetInt32("requestId").Value;
            return View(_closeCaseService.GetDetails(requestId));
        }

        [Authorization(24, "Admin")]
        public IActionResult ProviderLocation()
        {
            return View();
        }

        [Authorization(18, "Admin")]
        public IActionResult Invoice()
        {
            return View(_providersService.GetInvoiceDetails(0,null));
        }

        [Authorization("Admin", "Physician")]
        [HttpGet("/Case/ViewDocument")]
        public IActionResult ViewDocument()
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            int requestId = HttpContext.Session.GetInt32("requestId").Value;
            return View(_viewDocumentsServices.GetDocumentList(requestId: requestId, aspNetUserId: aspNetUserId));
        }

        [Authorization("Admin", "Physician")]
        [HttpGet("/Order/SendOrder")]
        public IActionResult SendOrder()
        {
            int requestId = HttpContext.Session.GetInt32("requestId").Value;
            return View(_sendOrderService.GetSendOrderDetails(requestId));
        }

        [Authorization("Admin")]
        public IActionResult UpdateBusiness(int venderId)
        {
            HttpContext.Session.SetInt32("venderId", venderId);
            return View("BusinessProfile", _partnersService.AddBusiness(isUpdate: true, venderId: venderId));
        }

        [Authorization("Admin")]
        public IActionResult UpdateRole(int roleId)
        {
            HttpContext.Session.SetInt32("roleId", roleId);
            return View("CreateRole", _accessService.GetEditRole(roleId));
        }

        [Authorization("Admin", "Physician")]
        public IActionResult SetPhyscianId(int physicianId)
        {
            HttpContext.Session.SetInt32("physicianId", physicianId);
            return RedirectToAction("EditProvider", "Admin");
        }

        public JsonResult OpenChat(int userId)
        {
            HttpContext.Session.SetInt32("userId", userId);
            return Json(new { redirect = Url.Action("Chat", "Admin") });
        }

        [Authorization("Admin", "Physician")]
        [HttpGet("/Profile/ViewProfile")]
        public IActionResult EditProvider()
        {
            int physicianId = HttpContext.Session.GetInt32("physicianId").Value;
            return View(_providersService.GetEditProvider(physicianId));
        }

        public IActionResult Agreement(String token)
        {
            Agreement agreement = _adminDashboardService.GetUserDetails(token);
            if (agreement.IsValid)
            {
                return View(agreement);
            }
            _notyfService.Error(agreement.Message);
            return RedirectToAction("PatientSite", "Patient");
        }

        public async Task<JsonResult> DeleteAllFiles(String requestWiseFileIdsList)   // delete all selected file - view documents
        {
            int requestId = HttpContext.Session.GetInt32("requestId").Value;
            if (await _viewDocumentsServices.DeleteAllFile(requestWiseFileIdsList, requestId))
            {
                _notyfService.Success("Successfully File Deleted");
            }
            else
            {
                _notyfService.Error("Failed!");
            }
            return Json(new { redirect = Url.Action("ViewDocument", "Admin") });
        }

        public async Task<IActionResult> DeleteFile(int requestWiseFileId)   /// delete particulate one file - view documents
        {
            if (await _viewDocumentsServices.DeleteFile(requestWiseFileId))
            {
                _notyfService.Success("Successfully File Deleted");
            }
            else
            {
                _notyfService.Error("Failed!");
            }
            return RedirectToAction("ViewDocument", "Admin");
        }

        public IActionResult SendMail(String requestWiseFileIdsList)  /// send mail from view document
        {
            int requestId = HttpContext.Session.GetInt32("requestId").Value;
            if (_viewDocumentsServices.SendFileMail(requestWiseFileIdsList, requestId))
            {
                _notyfService.Success("Successfully Send Mail");
            }
            else
            {
                _notyfService.Error("Failed!");
            }
            return Json(new { redirect = Url.Action("ViewDocument", "Admin") });
        }

        public async Task<IActionResult> CancelPopUp(CancelPopUp model)
        {
            if (await _viewNotesService.CancelRequest(model))
            {
                _notyfService.Success("Successfully Request Cancel");
            }
            else
            {
                _notyfService.Error("Request Cancel Failed!");
            }
            return RedirectToAction("Dashboard", "Admin");
        }

        public async Task<IActionResult> AssignPopUp(AssignAndTransferPopUp model)
        {
            if (await _viewNotesService.AssignRequest(model))
            {
                _notyfService.Success("Successfully Request Assign");
            }
            else
            {
                _notyfService.Error("Request Assign Failed!");
            }
            return RedirectToAction("Dashboard", "Admin");
        }

        public async Task<IActionResult> TransferPopUp(AssignAndTransferPopUp model)
        {
            if (ModelState.IsValid)
            {
                if (await _viewNotesService.AssignRequest(model))
                {
                    _notyfService.Success("Successfully Request Transfer");
                }
                else
                {
                    _notyfService.Error("Request Transfer Failed!");
                }
                return RedirectToAction("Dashboard", "Admin");
            }
            return View(model);
        }

        public async Task<IActionResult> BlockPopUp(BlockPopUp model)
        {
            if (ModelState.IsValid)
            {
                if (await _viewNotesService.BlockRequest(model))
                {
                    _notyfService.Success("Successfully Request Block");
                }
                else
                {
                    _notyfService.Error("Request Block Failed!");
                }
                return RedirectToAction("Dashboard", "Admin");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> ClearPopUp(int requestId)
        {
            if (await _viewNotesService.ClearRequest(requestId))
            {
                _notyfService.Success("Successfully Request Clear");
            }
            else
            {
                _notyfService.Error("Request Clear Failed !!");
            }
            return Json(new { redirect = Url.Action("Dashboard", "Admin") });
        }

        public IActionResult SendAgreementPopUp(Agreement model)
        {
            if (_viewNotesService.SendAgreement(model, HttpContext))
            {
                _notyfService.Success("Successfully Send");
            }
            else
            {
                _notyfService.Error("Agreement Send Failed!");
            }
            string role = HttpContext.Session.GetString("role");
            return RedirectToAction("Dashboard", role);
        }

        public IActionResult RequestSupport(RequestSupport model)  //// request support on dashboard
        {
            if (ModelState.IsValid)
            {
                if (_adminDashboardService.RequestSupport(model))
                {
                    _notyfService.Success("Successfully Send");
                }
                else
                {
                    _notyfService.Error("Failed!");
                }
                return RedirectToAction("Dashboard", "Admin");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShift(CreateShift model)  //// request support on dashboard
        {
            if (ModelState.IsValid)
            {
                int aspNetUseId = HttpContext.Session.GetInt32("aspNetUserId").Value;
                bool isAdmin = HttpContext.Session.GetString("role") == "Admin";
                if (await _providersService.CreateShift(model, aspNetUseId, isAdmin))
                {
                    _notyfService.Success("Successfully Created");
                }
                else
                {
                    _notyfService.Error("Failed!");
                }
                return isAdmin ? RedirectToAction("ProviderScheduling", "Admin") : RedirectToAction("Scheduling", "Physician");
            }
            return View(model);
        }

        public async Task<IActionResult> AgreementAgree(Agreement model)
        {
            if (await _viewNotesService.AgreementAgree(model))
            {
                _notyfService.Success("Successfully Agreed");
            }
            else
            {
                _notyfService.Error("Failed!");
            }
            return RedirectToAction("PatientSite", "Patient");
        }

        public async Task<IActionResult> AgreementDeclined(Agreement model)
        {
            if (await _viewNotesService.AgreementDeclined(model))
            {
                _notyfService.Success("Successfully Declined");
            }
            else
            {
                _notyfService.Error("Failed!");
            }
            return RedirectToAction("PatientSite", "Patient");
        }

        [HttpGet]  /// set requestId in session
        public JsonResult SetRequestId(int requestId, String actionName)
        {
            HttpContext.Session.SetInt32("requestId", requestId);
            return Json(new { redirect = Url.Action(actionName, "Admin") });
        }

        [HttpGet]  /// Navigation to Patient Records
        public JsonResult GetRecords(int userId)
        {
            HttpContext.Session.SetInt32("userId", userId);
            return Json(new { redirect = Url.Action("PatientRecord", "Admin") });
        }

        [HttpGet]  ////  SendAgreementPopUp
        public JsonResult GetEmailAndMobileNumber(int requestId)
        {
            return Json(_adminDashboardService.GetRequestClientEmailAndMobile(requestId));
        }

        [HttpGet] //// Assign case and TransfercasePopUp
        public JsonResult GetPhysicians(int regionId)
        {
            return Json(_adminDashboardService.GetPhysiciansByRegion(regionId));
        }

        [HttpGet] //// Send Order
        public JsonResult GetBusinesses(int professionId)
        {
            return Json(_sendOrderService.GetBusinessByProfession(professionId));
        }

        [HttpGet] //// Region Filter on Provider page
        public IActionResult RegionFilter(int regionId)
        {
            return PartialView("_ProviderTable", _providersService.GetProviders(regionId: regionId).Providers);
        }

        [HttpGet] //// change check box menus on create role page
        public IActionResult ChangeMenusByRole(int roleId)
        {
            return PartialView("_CreateRoleCheckBox", _accessService.GetMenusByRole(roleId));
        }

        [HttpPost] //// create roles
        public async Task<IActionResult> CreateRole(CreateRole model)
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            if (await _accessService.CreateRole(model, aspNetUserId))
            {
                _notyfService.Success("Successfully Role Created");
            }
            else
            {
                _notyfService.Error("Failed !!");
            }
            return RedirectToAction("Access", "Admin");
        }

        [HttpPost] //// update roles
        public async Task<IActionResult> UpdateRole(CreateRole model)
        {
            int roleId = HttpContext.Session.GetInt32("roleId").Value;
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            if (await _accessService.EditRole(model, roleId, aspNetUserId))
            {
                _notyfService.Success("Successfully Role Updated");
            }
            else
            {
                _notyfService.Error("Failed !!");
            }
            return RedirectToAction("Access", "Admin");
        }

        public async Task<IActionResult> DeleteRole(int roleId)  ////   delete role - access page
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            string result = await _accessService.DeleteRole(roleId, aspNetUserId);
            if (result == "")
            {
                _notyfService.Success("Successfully Role Deleted");
            }
            else
            {
                _notyfService.Error(result);
            }
            return RedirectToAction("Access", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]  /////  send link ---  Dashboard
        public IActionResult SendLink(SendLink model)
        {
            if (_adminDashboardService.SendRequestLink(model, HttpContext))
            {
                _notyfService.Success("Successfully Link Send");
            }
            else
            {
                _notyfService.Error("Link Send Failed !!");
            }
            string role = HttpContext.Session.GetString("role");
            return RedirectToAction("Dashboard", role);
        }

        [HttpPost("/Request/CreateRequest")]
        [ValidateAntiForgeryToken]  /////  create request ---  Dashboard
        public async Task<IActionResult> CreateRequest(CreateRequest model)
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            bool isAdmin = HttpContext.Session.GetString("role") == "Admin";
            if (await _adminDashboardService.CreateRequest(model, aspNetUserId, isAdmin))
            {
                _notyfService.Success("Successfully Request Added");
            }
            else
            {
                _notyfService.Error("Failed!");
            }
            string role = HttpContext.Session.GetString("role");
            return RedirectToAction("Dashboard", role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]  /////  create provider ---  provider page
        public async Task<IActionResult> CreateProvider(CreateProvider model)
        {
            if (ModelState.IsValid)
            {
                string result = await _providersService.CreateProvider(model);
                if (result == "")
                {
                    _notyfService.Success("Successfully Provider Created");
                }
                else
                {
                    _notyfService.Error(result);
                }
                return RedirectToAction("Providers", "Admin");
            }
            CreateProvider createProvider = _providersService.GetCreateProvider();
            model.Roles = createProvider.Roles;
            model.Regions = createProvider.Regions;
            _notyfService.Warning("Add Required Field.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]  /////  create Admin ---  dashboard 
        public async Task<IActionResult> CreateAdmin(AdminCreateAndProfile model)
        {
            if (ModelState.IsValid)
            {
                string result = await _accessService.CreateAdmin(model);
                if (result == "")
                {
                    _notyfService.Success("Successfully Admin Created");
                }
                else
                {
                    _notyfService.Error(result);
                }
                return RedirectToAction("Dashboard", "Admin");
            }
            AdminCreateAndProfile adminCreaateAndProfile = _accessService.GetAdminCreateAndProfile();
            model.Roles = adminCreaateAndProfile.Roles;
            model.Regions = adminCreaateAndProfile.Regions;
            _notyfService.Warning("Add Required Field.");
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost("/Case/ViewDocument")]
        public async Task<IActionResult> ViewDocument(ViewDocument model)
        {
            String firstName = HttpContext.Session.GetString("firstName");
            String lastName = HttpContext.Session.GetString("lastName");
            int requestId = HttpContext.Session.GetInt32("requestId").Value;
            if (await _viewDocumentsServices.UploadFile(model, firstName, lastName, requestId))
            {
                _notyfService.Success("Successfully File Added.");
            }
            else
            {
                _notyfService.Error("Failed !!");
            }
            return RedirectToAction("ViewDocument", "Admin");
        }

        [ValidateAntiForgeryToken]
        [HttpPost("/Case/ConcludeCare")]
        public async Task<IActionResult> ConcludeCare(ViewDocument model)
        {
            String firstName = HttpContext.Session.GetString("firstName");
            String lastName = HttpContext.Session.GetString("lastName");
            int requestId = HttpContext.Session.GetInt32("requestId").Value;
            if (await _viewDocumentsServices.UploadFile(model, firstName, lastName, requestId))
            {
                _notyfService.Success("Successfully File Added.");
            }
            else
            {
                _notyfService.Error("Failed !!");
            }
            return RedirectToAction("ConcludeCare", "Admin");
        }

        [ValidateAntiForgeryToken]
        [HttpPost("/Order/SendOrder")]
        public async Task<IActionResult> SendOrder(SendOrder model)
        {
            if (ModelState.IsValid)
            {
                int requestId = HttpContext.Session.GetInt32("requestId").Value;
                if (await _sendOrderService.AddOrderDetails(model, requestId))
                {
                    _notyfService.Success("Successfully Order Added");
                }
                else
                {
                    _notyfService.Error("Order Failed !!");
                }
                string role = HttpContext.Session.GetString("role");
                return RedirectToAction("Dashboard", role);
            }
            return View(model);
        }

        [HttpPost("/Encounter/EncounterForm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EncounterForm(EncounterForm model)
        {
            if (ModelState.IsValid)
            {
                int requestId = HttpContext.Session.GetInt32("requestId").Value;
                int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
                if (await _adminDashboardService.UpdateEncounter(model, requestId, aspNetUserId))
                {
                    _notyfService.Success("Successfully Updated");
                }
                else
                {
                    _notyfService.Error("Update Failed !!");
                }
                return RedirectToAction("Dashboard", "Admin");
            }
            _notyfService.Warning("Add Required Field.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]  //  login for physician and admin both
        public IActionResult LoginPage(Login model)
        {
            if (ModelState.IsValid)
            {
                UserDataModel user = _loginService.Auth(model, new List<int> { 2, 3 });
                if (!user.IsValid)
                {
                    _notyfService.Error(user.Message);
                    return View(null);
                }
                else
                {
                    HttpContext.Session.SetInt32("aspNetUserId", user.AspNetUserId);
                    HttpContext.Session.SetString("role", user.UserType);
                    HttpContext.Session.SetString("firstName", user.FirstName);
                    HttpContext.Session.SetString("lastName", user.LastName);
                    string token = _jwtService.GenrateJwtToken(user);
                    CookieOptions cookieOptions = new CookieOptions()
                    {
                        Secure = true,
                        Expires = DateTime.Now.AddMinutes(20),
                    };
                    Response.Cookies.Append("jwtToken", token, cookieOptions);
                    _notyfService.Success(user.Message);
                    return RedirectToAction("Dashboard", user.UserType);
                }
            }
            return View(null);
        }

        [HttpPost("/Case/ViewCase")]
        public async Task<IActionResult> ViewCase(ViewCase model)
        {
            if (await _adminDashboardService.UpdateRequest(model))
            {
                _notyfService.Success("Successfully Updated");
            }
            else
            {
                _notyfService.Error("Update Failed");
            }
            return RedirectToAction("ViewCase", "Admin");
        }

        [HttpPost]
        public async Task<IActionResult> ContactProvider(ContactProvider model)
        {
            if (await _providersService.ContactProvider(model))
            {
                _notyfService.Success("Successfully Message Send");
            }
            else
            {
                _notyfService.Error("Message Send Failed");
            }
            return RedirectToAction("Providers", "Admin");
        }

        [HttpPost]
        public async Task<IActionResult> CloseCase(CloseCase model)
        {
            int requestId = HttpContext.Session.GetInt32("requestId").Value;
            if (await _closeCaseService.UpdateDetails(model, requestId))
            {
                _notyfService.Success("Successfully Updated");
            }
            else
            {
                _notyfService.Error("Update Failed");
            }
            return RedirectToAction("CloseCase", "Admin");
        }

        public async Task<IActionResult> RequestAddToCloseCase()    ///  from close case page button click
        {
            int requestId = HttpContext.Session.GetInt32("requestId").Value;
            if (await _closeCaseService.RequestAddToCloseCase(requestId))
            {
                _notyfService.Success("Successfully Closed");
            }
            else
            {
                _notyfService.Error("Failed");
            }
            return RedirectToAction("Dashboard", "Admin");
        }

        [HttpGet]   // reset password through view profile
        public async Task<IActionResult> ViewProfileEditPassword(String newPassword)
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            if (await _loginService.ChangePassword(aspNetUserId, newPassword))
            {
                _notyfService.Success("Successfully Updated");
            }
            else
            {
                _notyfService.Error("Failed");
            }
            return Json(new { redirect = Url.Action("ViewProfile", "Admin") });
        }

        [HttpGet]   //  Edit Administrator Information view profile
        public async Task<IActionResult> EditAdministratorInformation(String data1, String firstName, String lastName)
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            if (await _viewProfileService.EditEditAdministratorInformation(data1, aspNetUserId))
            {
                HttpContext.Session.SetString("firstName", firstName);
                HttpContext.Session.SetString("lastName", lastName);
                _notyfService.Success("Successfully Updated");
            }
            else
            {
                _notyfService.Error("Failed");
            }
            return Json(new { redirect = Url.Action("ViewProfile", "Admin") });
        }

        [HttpGet]   //  Edit Mailing And Billing Information view profile
        public async Task<IActionResult> EditMailingAndBillingInformation(String data)
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            if (await _viewProfileService.EditMailingAndBillingInformation(data, aspNetUserId))
            {
                _notyfService.Success("Successfully Updated");
            }
            else
            {
                _notyfService.Error("Failed");
            }
            return Json(new { redirect = Url.Action("ViewProfile", "Admin") });
        }

        [HttpGet]   //  Edit Mailing And Billing Information view profile
        public async Task<IActionResult> EditProviderNotification(int physicianId, bool isNotification)
        {
            return Json(new { result = await _providersService.EditProviderNotification(physicianId, isNotification) });
        }

        [HttpGet]   // Export All Data 
        public IActionResult ExportAllData()
        {
            DataTable dataTable = _adminDashboardService.ExportAllData();
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AllData.xlsx");
                }
            }
        }

        [HttpGet]   // Export selected Data 
        public IActionResult ExportData(String status, int pageNo, String patientName, int regionId, int requesterTypeId)
        {
            DataTable dataTable = _adminDashboardService.ExportData(status, pageNo, patientName, regionId, requesterTypeId);
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Data.xlsx");
                }
            }
        }

        [HttpGet]   // Dashboard 
        public IActionResult GetTablesData(String status, int pageNo, String partialViewName, String patientName, int regionId, int requesterTypeId)
        {
            TableModel tableModel = _adminDashboardService.GetNewRequest(status, pageNo, patientName, regionId, requesterTypeId);
            return tableModel.TableDatas.Count != 0 ? PartialView(partialViewName, tableModel) : PartialView("_NoTableDataFound");
        }

        [HttpPost]    // Add Business - BusinessProfile page
        public async Task<IActionResult> CreateBusiness(BusinessProfile model)
        {
            if (await _partnersService.CreateBusiness(model))
            {
                _notyfService.Success("Successfully Business Added");
            }
            else
            {
                _notyfService.Error("Add Business Failed");
            }
            return RedirectToAction("Partners", "Admin");
        }

        [HttpPost]    // update Business - BusinessProfile page
        public async Task<IActionResult> UpdateBusiness(BusinessProfile model)
        {
            int venderId = HttpContext.Session.GetInt32("venderId").Value;
            if (await _partnersService.EditBusiness(model, venderId))
            {
                _notyfService.Success("Successfully Business Updated");
                HttpContext.Session.Remove("venderId");
            }
            else
            {
                _notyfService.Error("Update Business Failed");
            }
            return RedirectToAction("Partners", "Admin");
        }

        public async Task<IActionResult> DeleteBusiness(int venderId)   // Delete Business - BusinessProfile page
        {
            if (await _partnersService.DeleteBusiness(venderId))
            {
                _notyfService.Success("Successfully Business Deleted");
            }
            else
            {
                _notyfService.Error("Delete Business Failed");
            }
            return RedirectToAction("Partners", "Admin");
        }

        [HttpPost("/Case/ViewNotes")]
        public async Task<IActionResult> ViewNotes(ViewNotes model)
        {
            if (ModelState.IsValid)
            {
                int requestId = HttpContext.Session.GetInt32("requestId").Value;
                int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
                bool isAdmin = HttpContext.Session.GetString("role") == "Admin";
                if (await _viewNotesService.AddAdminNotes(model.NewNotes, requestId, aspNetUserId, isAdmin))
                {
                    _notyfService.Success("Successfully Notes Added");
                }
                else
                {
                    _notyfService.Error("Add Notes Failed");
                }
            }
            return RedirectToAction("ViewNotes", "Admin");
        }

        [HttpGet]    // Send Order
        public HealthProfessional GetBusinessData(int venderId)
        {
            return _sendOrderService.GetBusinessData(venderId);
        }

        [HttpGet]    // provider scheduling page 
        public IActionResult ChangeTab(string name, int regionId, int type, String time)
        {
            return type switch
            {
                1 or 2 => PartialView(name, _providersService.GetSchedulingTableDate(regionId, type, time)), ///  for case 1 and 2 function is same
                3 or _ => PartialView(name, _providersService.MonthWiseScheduling(regionId, time)),
            };
        }

        [HttpGet]    // RequestShift page  
        public IActionResult GetRequestShiftTableData(int regionId, bool isMonth, int pageNo)
        {
            return PartialView("_RequestedShiftTable", _providersService.GetRequestShiftTableDate(regionId, isMonth, pageNo));
        }

        [HttpGet]    // RequestShift page  
        public async Task<IActionResult> UpdateShiftDetails(string data, bool isApprove)
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            if (await _providersService.ChangeShiftDetails(data, isApprove, aspNetUserId))
            {
                if (isApprove)
                {
                    _notyfService.Success("Successfully Approved");
                }
                else
                {
                    _notyfService.Success("Successfully Deleted");
                }
            }
            else
            {
                _notyfService.Error("Failed!");
            }
            return Json(new { redirect = Url.Action("RequestedShift", "Admin") });
        }

        [HttpGet]       ///  provider Location page 
        public IActionResult GetProviderLocation()
        {
            return Json(_providersService.GetProviderLocation());
        }

        [HttpGet]        ///   Provider Scheduling page
        public IActionResult GetShiftDetails(int shiftDetailsId)
        {
            return Json(_providersService.GetShiftDetails(shiftDetailsId));
        }

        [HttpGet]     // Edit Shift - provider Scheduling page
        public async Task<JsonResult> EditShiftDetails(string data)
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            await _providersService.EditShiftDetails(data, aspNetUserId);
            return Json(new { redirect = Url.Action("ProviderScheduling", "Admin") });
        }

        [HttpGet]     // Delete Shift - provider Scheduling page
        public async Task<JsonResult> DeleteShiftDetails(string data)
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            await _providersService.ChangeShiftDetails(data, false, aspNetUserId);    ///  use same services from requested shift page for delete shift
            return Json(new { redirect = Url.Action("ProviderScheduling", "Admin") });
        }

        [HttpGet]    // Partners page 
        public IActionResult GetPartnersData(int regionId, string searchElement)
        {
            return PartialView("_PartnersTable", _partnersService.GetPartnersTableDatas(regionId, searchElement));
        }

        [HttpPost]    // Record page filters
        public IActionResult GetRecordsTableDate(Records model)
        {
            return PartialView("_RecordTable", _recordService.GetRecords(model).RecordTableDatas);
        }

        [HttpGet]   // Export All Data  -- Record Page
        public IActionResult ExportAllRecords()
        {
            DataTable dataTable = _recordService.ExportAllRecords();
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AllData.xlsx");
                }
            }
        }

        [HttpPost]    // Email Logs page filters
        public IActionResult GetEmailLogsTableDate(EmailSmsLogs model)
        {
            return PartialView("_EmailLogTable", _recordService.GetEmailLogTabledata(model));
        }

        [HttpPost]    // SMS Logs page filters
        public IActionResult GetSMSLogsTableDate(EmailSmsLogs model)
        {
            return PartialView("_SmsLogTable", _recordService.GetSMSLogTabledata(model));
        }

        [HttpPost]    // Patient History filters
        public IActionResult GetPatientHistoryTableDate(string model, int pageNo)
        {
            return PartialView("_PatientHistoryTable", _recordService.GetPatientHistoryTable(model, pageNo));
        }

        [HttpGet]    // Provider On call Filter
        public IActionResult GetProviderOnCall(int regionId)
        {
            return PartialView("_ProviderOnCallTable", _providersService.GetProviderList(regionId));
        }

        [HttpGet]    // Provider Record page 
        public IActionResult GetPatientRecord(int pageNo)
        {
            int userId = HttpContext.Session.GetInt32("userId").Value;
            return PartialView("_PatientRecordTable", _recordService.GetPatientRecord(userId, pageNo));
        }

        [HttpPost]    // Block History filters
        public IActionResult GetBlockHistoryTableDate(string model, int pageNo, string date)
        {
            return PartialView("_BlockHistoryTable", _recordService.GetBlockHistoryTable(model, pageNo, date));
        }

        [HttpPost]    // Block History page - unblock Request
        public async Task<JsonResult> UnblockRequest(int requestId)
        {
            if (await _recordService.UnblockRequest(requestId))
            {
                _notyfService.Success("Successfully UnBlocked");
            }
            else
            {
                _notyfService.Error("UnBlock Failed");
            }
            return Json(new { redirect = Url.Action("BlockHistory", "Admin") });
        }

        [HttpPost]    //  edit provider account information
        public async Task<IActionResult> EditPhysicianAccountInformation(EditProvider model)
        {
            int physicianId = HttpContext.Session.GetInt32("physicianId").Value;
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            if (await _providersService.EditPhysicianAccountInformation(model, physicianId, aspNetUserId))
            {
                _notyfService.Success("Successfully Updated");
            }
            else
            {
                _notyfService.Error("Failed");
            }
            return RedirectToAction("EditProvider", "Admin");
        }

        [HttpPost]    //  edit provider Physician Information
        public async Task<IActionResult> EditPhysicianPhysicianInformation(EditProvider model)
        {
            int physicianId = HttpContext.Session.GetInt32("physicianId").Value;
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            if (await _providersService.EditPhysicianPhysicianInformation(model, physicianId, aspNetUserId))
            {
                _notyfService.Success("Successfully Updated");
            }
            else
            {
                _notyfService.Error("Failed");
            }
            return RedirectToAction("EditProvider", "Admin");
        }

        [HttpPost]    //  edit provider Mailing & Billing Information
        public async Task<IActionResult> EditPhysicianMailAndBillingInformation(EditProvider model)
        {
            int physicianId = HttpContext.Session.GetInt32("physicianId").Value;
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            if (await _providersService.EditPhysicianMailAndBillingInformation(model, physicianId, aspNetUserId))
            {
                _notyfService.Success("Successfully Updated");
            }
            else
            {
                _notyfService.Error("Failed");
            }
            return RedirectToAction("EditProvider", "Admin");
        }

        [HttpPost]    //  edit provider Provider Profile
        public async Task<IActionResult> EditPhysicianProviderProfile(EditProvider model)
        {
            int physicianId = HttpContext.Session.GetInt32("physicianId").Value;
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            if (await _providersService.EditPhysicianProviderProfile(model, physicianId, aspNetUserId))
            {
                _notyfService.Success("Successfully Updated");
            }
            else
            {
                _notyfService.Error("Failed");
            }
            return RedirectToAction("EditProvider", "Admin");
        }

        [HttpPost]    //  edit provider Onbording information
        public async Task<IActionResult> EditPhysicianOnbordingInformation(EditProvider model)
        {
            int physicianId = HttpContext.Session.GetInt32("physicianId").Value;
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            if (await _providersService.EditPhysicianOnbordingInformation(model, physicianId, aspNetUserId))
            {
                _notyfService.Success("Successfully Updated");
            }
            else
            {
                _notyfService.Error("Failed");
            }
            return RedirectToAction("EditProvider", "Admin");
        }

        [HttpPost]    //  save Signature form edit Provider
        public async Task<JsonResult> SaveSignature(string file)
        {
            int physicianId = HttpContext.Session.GetInt32("physicianId").Value;
            if (await _providersService.SaveSign(file, physicianId))
            {
                _notyfService.Success("Successfully Updated");
            }
            else
            {
                _notyfService.Error("Failed");
            }
            return Json(new { redirect = Url.Action("EditProvider", "Admin") });
        }

        public JsonResult GetInvoice(int physicianId, string date)
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            return Json(_providersService.GetInvoiceDetails(physicianId,date));
        }

        public IActionResult GetWeeklyTimeSheet(int physicianId, string date)
        {
            return PartialView("_WeeklyTimeSheet", _providersService.GetWeeklyTimeSheet(physicianId,date));
        }

        public async Task<JsonResult> ApproveInvoice(int invoiceId, double totalAmount, double bounsAmount, string notes)
        {
            await _providersService.ApproveInvoice(invoiceId, totalAmount, bounsAmount, notes); 
            return Json(new { redirect = Url.Action("Invoice", "Admin") });
        }

        public IActionResult GetReceipts(int physicianId, string date)
        {
            return PartialView("_ReciptsTableData", _providersService.GetReceipts(physicianId,date));
        }
        
        public async Task<IActionResult> EditPayRate(PayRate model)
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            int physicianId = HttpContext.Session.GetInt32("physicianId").Value;
            await _providersService.EditPayRate(model, physicianId, aspNetUserId);
            return RedirectToAction("PayRate", "Admin");
        }
    }
}
