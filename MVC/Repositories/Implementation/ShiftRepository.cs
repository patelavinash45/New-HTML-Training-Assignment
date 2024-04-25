using Microsoft.EntityFrameworkCore;
using Repositories.DataContext;
using Repositories.DataModels;
using Repositories.Interfaces;
using System.Collections;

namespace Repositories.Implementation
{
    public class ShiftRepository : IShiftRepository
    {
        private readonly HalloDocDbContext _dbContext;

        public ShiftRepository(HalloDocDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Physician> GetPhysicianWithShiftDetailByRegionIdAndDAte(int regionId, DateTime startDate, DateTime endDate)
        {
            return _dbContext.Physicians.Include(a => a.Shifts).ThenInclude(a => a.ShiftDetails.Where(
                        a => (regionId == 0 || a.RegionId == regionId)
                             && a.IsDeleted == new BitArray(1, false)
                             && a.ShiftDate.Date >= startDate.Date && a.ShiftDate.Date <= endDate.Date)).ToList();
        }

        public List<ShiftDetail> GetShiftDetailByPhysicianIdAndDAte(int aspNetUserId, DateTime startDate, DateTime endDate)
        {
            Func<ShiftDetail, bool> predicate = a =>
            a.Shift.Physician.AspNetUserId == aspNetUserId
            && !a.IsDeleted[0]
            && a.ShiftDate.Date >= startDate.Date && a.ShiftDate.Date <= endDate.Date;
            return _dbContext.ShiftDetails.Include(a => a.Shift).ThenInclude(a => a.Physician).Where(predicate).ToList();
        }

        public List<ShiftDetail> GetShiftDetailByRegionIdAndDAte(int regionId, DateTime startDate, DateTime endDate)
        {
            Func<ShiftDetail, bool> predicate = a =>
            (regionId == 0 || a.RegionId == regionId)
            && !a.IsDeleted[0]
            && a.ShiftDate.Date >= startDate.Date && a.ShiftDate.Date <= endDate.Date;
            return _dbContext.ShiftDetails.Include(a => a.Shift).ThenInclude(a => a.Physician).Where(predicate).ToList();
        }

        public List<ShiftDetail> GetAllShiftDetailsFromShiftId(int shiftId)
        {
            return _dbContext.ShiftDetails.Where(a => a.ShiftId == shiftId).ToList();
        }

        public async Task<bool> AddShift(Shift shift)
        {
            _dbContext.Shifts.Add(shift);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddShiftDetails(List<ShiftDetail> shiftDetails)
        {
            _dbContext.ShiftDetails.AddRange(shiftDetails);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddShiftDetailsRegion(List<ShiftDetailRegion> shiftDetailRegions)
        {
            _dbContext.ShiftDetailRegions.AddRange(shiftDetailRegions);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateShiftDetails(List<ShiftDetail> shiftDetails)
        {
            _dbContext.ShiftDetails.UpdateRange(shiftDetails);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateShiftDetailRegions(List<ShiftDetailRegion> shiftDetailRegions)
        {
            _dbContext.ShiftDetailRegions.UpdateRange(shiftDetailRegions);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public ShiftDetail GetShiftDetails(int shiftDetailsId)
        {
            return _dbContext.ShiftDetails.FirstOrDefault(a => a.ShiftDetailId == shiftDetailsId);
        }

        public ShiftDetailRegion GetShiftDetailRegion(int shiftDetailsId)
        {
            return _dbContext.ShiftDetailRegions.FirstOrDefault(a => a.ShiftDetailId == shiftDetailsId);
        }

        public ShiftDetail GetShiftDetailsWithPhysician(int shiftDetailsId)
        {
            return _dbContext.ShiftDetails.Include(a => a.Shift.Physician).Include(a => a.Region)
                                                                        .FirstOrDefault(a => a.ShiftDetailId == shiftDetailsId);
        }

        public List<ShiftDetail> GetAllShiftDetails(int regionId, bool isThisMonth, DateTime date, int skip)
        {
            Func<ShiftDetail,bool>  predicate = a => 
            (!isThisMonth || (a.ShiftDate.Date >= date.Date && a.ShiftDate.Date <= date.AddMonths(1).Date)) 
            && (regionId == 0 || a.RegionId == regionId)
            && !a.IsDeleted[0]
            && a.Status == 0;
            return _dbContext.ShiftDetails.Include(a => a.Shift).ThenInclude(a => a.Physician).Include(a => a.Region)
                   .Where(predicate).OrderByDescending(a => a.ShiftDate).Skip(skip).Take(10).ToList();
        }

        public int CountAllShiftDetails(int regionId, bool isThisMonth, DateTime date)
        {
            Func<ShiftDetail, bool> predicate = a =>
            (!isThisMonth || (a.ShiftDate.Date >= date.Date && a.ShiftDate.Date <= date.AddMonths(1).Date))
            && (regionId == 0 || a.RegionId == regionId)
            && !a.IsDeleted[0]
            && a.Status == 0;
            return _dbContext.ShiftDetails.Include(a => a.Region).Where(predicate).Count();
        }
    }
}
