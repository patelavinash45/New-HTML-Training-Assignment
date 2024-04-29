// Ignore Spelling: admin Rgions

using Microsoft.EntityFrameworkCore;
using Repositories.DataContext;
using Repositories.DataModels;
using Repositories.Interfaces;

namespace Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly HalloDocDbContext _dbContext;

        public UserRepository(HalloDocDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int CountUsers(Func<User, bool> predicat)
        {
            return _dbContext.Users.Where(predicat).Count();
        }

        public List<User> GetAllUser(Func<User, bool> predicat, int skip)
        {
            return _dbContext.Users.Where(predicat).OrderByDescending(a => a.UserId).Skip(skip).Take(5).ToList();
        }

        public int GetUserID(int aspNetUserID)
        {
            User user = _dbContext.Users.FirstOrDefault(a => a.AspNetUserId == aspNetUserID);
            return user?.UserId ?? 0;
        }

        public async Task<int> AddUser(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user?. UserId ?? 0;
        }

        public User GetUser(int aspNetUserID)
        {
            return _dbContext.Users.FirstOrDefault(a => a.AspNetUserId == aspNetUserID);
        }

        public async Task<bool> UpdateProfile(User user)
        {
            _dbContext.Users.Update(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public Admin GetAdmionByAspNetUserId(int aspNetUserId)
        {
            return _dbContext.Admins.FirstOrDefault(a => a.AspNetUserId == aspNetUserId);
        }

        public Physician GetPhysicianByAspNetUserId(int aspNetUserId)
        {
            return _dbContext.Physicians.FirstOrDefault(a => a.AspNetUserId == aspNetUserId);
        }

        public async Task<bool> AddAdmin(Admin admin)
        {
            _dbContext.Admins.Add(admin);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public List<Physician> GetAllPhysiciansByRegionId(int regionId)
        {
            return _dbContext.Physicians.Include(a => a.PhysicianNotifications).Where(a =>(regionId == 0 || a.RegionId == regionId)).ToList();
        }

        public List<PhysicianRegion> GetAllPhysicianRegionsByRegionId(int regionId)
        {
            return _dbContext.PhysicianRegions.Include(a => a.Physician).Where(a => (regionId == 0 || a.RegionId == regionId)).ToList();
        }

        public List<PhysicianRegion> GetAllPhysicianRegionsByPhysicianId(int physicianId)
        {
            return _dbContext.PhysicianRegions.Where(a => a.PhysicianId == physicianId).ToList();
        }

        public List<PhysicianRegion> GetAllPhysicianRegionsByAspNetUserIdWithRegionName(int aspNetUserId)
        {
            return _dbContext.PhysicianRegions.Include(a => a.Region).Include(a => a.Physician).Where(a => a.Physician.AspNetUserId == aspNetUserId).ToList();
        }

        public List<Physician> GetAllUnAssignedPhysician()
        {
            return _dbContext.Physicians.Where(a => a.Status==0).ToList();
        }

        public PhysicianNotification GetPhysicianNotification(int physicianId)
        {
            return _dbContext.PhysicianNotifications.FirstOrDefault(a => a.PhysicianId ==  physicianId);
        }

        public async Task<bool> UpdatePhysicianNotification(PhysicianNotification physicianNotification)
        {
            _dbContext.PhysicianNotifications.Update(physicianNotification);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public Physician GetPhysicianByPhysicianId(int physicianId)
        {
            return _dbContext.Physicians.FirstOrDefault(a => a.PhysicianId == physicianId);
        }

        public Physician GetPhysicianWithAspNetUser(int physicianId)
        {
            return _dbContext.Physicians.Include(a => a.AspNetUser).FirstOrDefault(a => a.PhysicianId == physicianId);
        }

        public List<AdminRegion> GetAdminRegionByAdminId(int adminId)
        {
            return _dbContext.AdminRegions.Where(a => a.AdminId == adminId).ToList();
        }

        public async Task<bool> UpdateAdmin(Admin admin)
        {
            _dbContext.Admins.Update(admin);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddAdminRgions(List<AdminRegion> adminRegions)
        {
            _dbContext.AdminRegions.AddRange(adminRegions);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAdminRgions(List<AdminRegion> adminRegions)
        {
            _dbContext.AdminRegions.RemoveRange(adminRegions);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddPhysicianRegions(List<PhysicianRegion> physicianRegions)
        {
            _dbContext.PhysicianRegions.AddRange(physicianRegions);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePhysicianRegions(List<PhysicianRegion> physicianRegions)
        {
            _dbContext.PhysicianRegions.RemoveRange(physicianRegions);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddPhysician(Physician physician)
        {
            _dbContext.Physicians.Add(physician);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdatePhysician(Physician physician)
        {
            _dbContext.Physicians.Update(physician);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddPhysicianNotification(PhysicianNotification physicianNotification)
        {
            _dbContext.PhysicianNotifications.Add(physicianNotification);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public List<PhysicianLocation> GetAllProviderLocation()
        {
            return _dbContext.PhysicianLocations.ToList();
        }

        public List<Physician> GetAllPhysicianWithCurrentShift(int regionId)
        {
            DateOnly date = DateOnly.FromDateTime(DateTime.Now);
            TimeOnly time = TimeOnly.FromDateTime(DateTime.Now);
            return _dbContext.Physicians.Include(a => a.Shifts).ThenInclude(a => a.ShiftDetails
                         .Where(a => (regionId == 0 || a.RegionId == regionId) && DateOnly.FromDateTime(a.ShiftDate) == date && a.StartTime <= time
                                                                  && a.EndTime >= time)).ToList();
        }
    }
}

