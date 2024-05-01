using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDoc.Auth;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces.AdminServices;
using Services.Interfaces.PhysicianServices;
using Services.ViewModels.Admin;
using Services.ViewModels.Physician;

namespace HelloDoc.Controllers
{
    public class PhysicianController : Controller
    {

        private readonly INotyfService _notyfService;
        private readonly IPhysicianDashboardService _physicianDashboardService;
        private readonly IAdminDashboardService _adminDashboardService;
        private readonly IInvoiceService _invoiceService;

        public PhysicianController(INotyfService notyfService, IPhysicianDashboardService physicianDashboardService,
                                                  IAdminDashboardService adminDashboardService, IInvoiceService invoiceService)
        {
            _notyfService = notyfService;
            _physicianDashboardService = physicianDashboardService;  
            _adminDashboardService = adminDashboardService; 
            _invoiceService = invoiceService;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorization(25,"Physician")]
        public IActionResult Dashboard()
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            return View(_physicianDashboardService.GetAllRequests(aspNetUserId));
        }

        [HttpGet]   // Dashboard 
        public IActionResult GetTablesData(String status, int pageNo, String partialViewName, String patientName, int regionId, int requesterTypeId)
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            TableModel tableModel = _physicianDashboardService.GetNewRequest(status, pageNo, patientName, regionId, requesterTypeId, aspNetUserId);
            return tableModel.TableDatas.Count != 0 ? PartialView(partialViewName, tableModel) : PartialView("_NoTableDataFound");
        }

        [Authorization(26, "Physician")]
        public IActionResult Scheduling()
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            return View(_physicianDashboardService.ProviderScheduling(aspNetUserId));
        }

        [Authorization(30, "Physician")]
        public IActionResult Invoice()
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            return View(_invoiceService.GetInvoice(aspNetUserId));
        }

        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            if (await _physicianDashboardService.AcceptRequest(requestId))
            {
                _notyfService.Success("Successfully Accepted");
            }
            else
            {
                _notyfService.Error("Failed !!");
            }
            return RedirectToAction("Dashboard", "Physician");
        }

        [HttpPost]
        public async Task<IActionResult> TransferRequest(PhysicianTransferRequest model)
        {
            if (await _physicianDashboardService.TransferRequest(model))
            {
                _notyfService.Success("Successfully Transfered");
            }
            else
            {
                _notyfService.Error("Failed !!");
            }
            return RedirectToAction("Dashboard", "Physician");
        }
            
        [HttpGet]
        public async Task<JsonResult> SetEncounter(bool isVideoCall,int requestId)
        {
            await _physicianDashboardService.SetEncounter(requestId, isVideoCall);
            return Json(new { redirect = Url.Action("Dashboard", "Physician") });
        }

        public async Task<IActionResult> CreateInvoice(CreateInvoice model)
        {
            return View();
        }

        public async Task<IActionResult> HomeVisit(int requestId)
        {
            if (await _physicianDashboardService.SetEncounter(requestId,true))
            {
                HttpContext.Session.SetInt32("requestId", requestId);
                return RedirectToAction("EncounterForm", "Admin");
            }
            else
            {
                return RedirectToAction("Dashboard", "Physician");
            }
        }

        public IActionResult ViewProfile()
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            int PhysicianId = _physicianDashboardService.GetPhysicianIdFromAspNetUserId(aspNetUserId);
            return RedirectToAction("SetPhyscianId", "Admin", new { physicianId = PhysicianId });
        }

        public async Task<IActionResult> FinalizeEncounter(EncounterForm model)
        {
            int requestId = HttpContext.Session.GetInt32("requestId").Value;
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            model.IsFinalize = true;
            if (await _adminDashboardService.UpdateEncounter(model, requestId, aspNetUserId))
            {
                _notyfService.Success("Successfully Updated");
            }
            else
            {
                _notyfService.Error("Update Failed !!");
            }
            return RedirectToAction("Dashboard", "Physician");
        }

        [HttpGet]    // provider scheduling page 
        public IActionResult GetMonthWiseData(String time)
        {
            int aspNetUserId = HttpContext.Session.GetInt32("aspNetUserId").Value;
            return PartialView("_monthWiseScheduling", _physicianDashboardService.MonthWiseScheduling(time, aspNetUserId));
        }

        public IActionResult GeneratePDF(int requestId)
        {
            byte[] bytes = _physicianDashboardService.GenerateMedicalReport(requestId);
            return File(bytes, "application/pdf", "MedicalReport.pdf");
        }

        [HttpPost("/Case/FinalConcludeCare")]
        public async Task<IActionResult> FinalConcludeCare(ConcludeCare model)
        {
            int requestId = HttpContext.Session.GetInt32("requestId").Value;
            if(await _physicianDashboardService.ConcludeCare(requestId, model))
            {
                _notyfService.Success("Successfully Concluded");
                return RedirectToAction("Dashboard", "Physician");
            }
            else
            {
                _notyfService.Warning("Encounter Is not Finalize");
                return RedirectToAction("EncounterForm", "Admin");
            }
        }

    }
}
