using Services.ViewModels;
using Services.ViewModels.Admin;

namespace Services.Interfaces
{
    public interface IViewProfileService
    {
        ViewProfile GetProfileDetails(int aspNetUserId);

        Task<bool> UpdatePatientProfile(ViewProfile model, int aspNetUserId);

        AdminCreateAndProfile GetAdminViewProfile(int aspNetUserId);

        Task<bool> EditEditAdministratorInformation(String data, int aspNetUserId);

        Task<bool> EditMailingAndBillingInformation(String data1, int aspNetUserId);
    }
}
