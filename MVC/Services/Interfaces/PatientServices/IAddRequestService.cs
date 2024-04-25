using Services.ViewModels;

namespace Services.Interfaces.PatientServices
{
    public interface IAddRequestService
    {
        bool IsEmailExists(String email);

        Dictionary<int, String> GetRegions();

        AddRequestByPatient GetModelForRequestByMe(int aspNetUserId);

        Task<bool> AddPatientRequest(AddPatientRequest model);

        Task<bool> AddRequestForMe(AddRequestByPatient model);

        Task<bool> AddRequestForSomeOneelse(AddRequestByPatient model, int aspNetUserIdMe);

        Task<bool> AddConciergeRequest(AddConciergeRequest model);

        Task<bool> AddFamilyFriendRequest(AddFamilyRequest model);

        Task<bool> AddBusinessRequest(AddBusinessRequest model);
    }
}
