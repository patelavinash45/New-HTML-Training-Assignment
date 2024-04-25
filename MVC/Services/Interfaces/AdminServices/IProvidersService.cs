using Services.ViewModels.Admin;

namespace Services.Interfaces.AdminServices
{
    public interface IProvidersService
    {
        List<ProviderLocation> GetProviderLocation();

        Provider GetProviders(int regionId);

        Task<bool> EditProviderNotification(int providerId, bool isNotification);

        Task<bool> ContactProvider(ContactProvider model);

        CreateProvider GetCreateProvider();

        EditProvider GetEditProvider(int physicianId);

        Task<String> CreateProvider(CreateProvider model);

        ProviderScheduling GetProviderSchedulingData();

        List<SchedulingTable> GetSchedulingTableDate(int regionId, int type, string date);

        SchedulingTableMonthWise MonthWiseScheduling(int regionId, String dateString);

        Task<bool> CreateShift(CreateShift model, int aspNetUserId, bool isAdmin);

        RequestedShift GetRequestedShift();

        RequestShiftModel GetRequestShiftTableDate(int regionId, bool isMonth, int pageNo);

        Task<bool> ChangeShiftDetails(string dataList, bool isApprove, int aspNetUserId);

        ViewShift GetShiftDetails(int shiftDetailsId);

        Task<bool> EditShiftDetails(string data, int aspNetUserId);

        ProviderOnCall GetProviderOnCall(int regionId);

        ProviderList GetProviderList(int regionId);

        Task<bool> SaveSign(string sign, int physicianId);

        Task<bool> EditphysicianAccountInformaction(EditProvider model, int physicianId, int aspNetUserId);

        Task<bool> EditphysicianPhysicianInformaction(EditProvider model, int physicianId, int aspNetUserId);

        Task<bool> EditphysicianMailAndBillingInformaction(EditProvider model, int physicianId, int aspNetUserId);

        Task<bool> EditphysicianProviderProfile(EditProvider model, int physicianId, int aspNetUserId);
        
        Task<bool> EditphysicianOnbordingInformaction(EditProvider model, int physicianId, int aspNetUserId);
    }
}
