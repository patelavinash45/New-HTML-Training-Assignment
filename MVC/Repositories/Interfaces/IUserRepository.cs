using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IUserRepository
    {
        int CountUsers(Func<User, bool> predicat);

        List<User> GetAllUser(Func<User, bool> predicat, int skip);

        int GetUserID(int aspNetUserID);

        Task<int> AddUser(User user);

        User GetUser(int aspNetUserID);

        Task<bool> UpdateProfile(User user);

        Admin GetAdmionByAspNetUserId(int aspNetUserId);

        Physician GetPhysicianByAspNetUserId(int aspNetUserId);

        Task<bool> AddAdmin(Admin admin);

        List<Physician> GetAllPhysiciansByRegionId(int regionId);

        List<PhysicianRegion> GetAllPhysicianRegionsByRegionId(int regionId);

        List<PhysicianRegion> GetAllPhysicianRegionsByPhysicianId(int physicianId);

        List<PhysicianRegion> GetAllPhysicianRegionsByAspNetUserIdWithRegionName(int aspNetUserId);

        List<Physician> GetAllUnAssignedPhysician();

        PhysicianNotification GetPhysicianNotification(int physicianId);

        Task<bool> UpdatePhysicianNotification(PhysicianNotification physicianNotification);

        Physician GetPhysicianByPhysicianId(int physicianId);

        Physician GetPhysicianWithAspNetUser(int physicianId);

        List<AdminRegion> GetAdminRegionByAdminId(int adminId);

        Task<bool> UpdateAdmin(Admin admin);

        Task<bool> AddAdminRgions(List<AdminRegion> adminRegions);

        Task<bool> DeleteAdminRgions(List<AdminRegion> adminRegions);

        Task<bool> AddPhysicianRegions(List<PhysicianRegion> physicianRegions);

        Task<bool> DeletePhysicianRegions(List<PhysicianRegion> physicianRegions);

        Task<bool> AddPhysician(Physician physician);

        Task<bool> UpdatePhysician(Physician physician);

        Task<bool> AddPhysicianNotification(PhysicianNotification physicianNotification);

        List<PhysicianLocation> GetAllProviderLocation();

        List<Physician> GetAllPhysicianWithCurrentShift(int regionId);

        PhysicianPayRate GetPhysicianPayRate(int physicianId);

        Task<bool> EditPhysicianPayRate(PhysicianPayRate physicianPayRate);
    }
}
