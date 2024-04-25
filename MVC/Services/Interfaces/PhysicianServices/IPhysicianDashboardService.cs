using Microsoft.AspNetCore.Mvc;
using Services.ViewModels.Admin;
using Services.ViewModels.Physician;

namespace Services.Interfaces.PhysicianServices
{
    public interface IPhysicianDashboardService
    {
        PhysicianDashboard GetallRequests(int aspNetUserId);

        TableModel GetNewRequest(String status, int pageNo, String patientName, int regionId, int requesterTypeId, int aspNetUserId);

        Task<bool> AcceptRequest(int requestId);

        Task<bool> TransferRequest(PhysicianTransferRequest model);

        Task<bool> SetEncounter(int requestId, bool isVideoCall);

        int GetPhysicianIdFromAspNetUserId(int aspNetUserId);

        byte[] GenerateMedicalReport(int requestId);

        Task<bool> ConcludeCare(int requestId, ConcludeCare model);

        PhysicianScheduling ProviderScheduling(int physicianId);

        SchedulingTableMonthWise MonthWiseScheduling(string dateString, int physicianId);
    }
}
