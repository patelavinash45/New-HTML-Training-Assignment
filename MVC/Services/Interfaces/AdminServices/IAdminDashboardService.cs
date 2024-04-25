using Microsoft.AspNetCore.Http;
using Services.ViewModels.Admin;
using Services.ViewModels.Physician;
using System.Data;

namespace Services.Interfaces.AdminServices
{
    public interface IAdminDashboardService
    {
        AdminDashboard GetallRequests();

        TableModel GetNewRequest(String status, int pageNo, String patientName, int regionId, int requesterTypeId);

        Dictionary<int, String> GetPhysiciansByRegion(int regionId);

        Tuple<String, String, int> GetRequestClientEmailAndMobile(int requestId);

        Agreement GetUserDetails(String token);

        DataTable ExportAllData();

        DataTable ExportData(String status, int pageNo, String patientName, int regionId, int requesterTypeId);

        bool SendRequestLink(SendLink model,HttpContext httpContext);

        Task<bool> CreateRequest(CreateRequest model, int aspNetUserId, bool isAdmin);

        bool RequestSupport(RequestSupport model);

        EncounterForm GetEncounterDetails(int requestId, bool isAdmin);

        Task<bool> UpdateEncounter(EncounterForm model, int requestId, int aspNetUserId);

        ViewCase GetRequestDetails(int requestId, bool isAdmin);

        Task<bool> UpdateRequest(ViewCase model);

        ConcludeCare GetConcludeCare(int requestId);
    }
}
