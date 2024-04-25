using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IShiftRepository
    {
        List<Physician> GetPhysicianWithShiftDetailByRegionIdAndDAte(int regionId, DateTime startDate, DateTime endDate);

        List<ShiftDetail> GetShiftDetailByPhysicianIdAndDAte(int aspNetUserId, DateTime startDate, DateTime endDate);

        List<ShiftDetail> GetShiftDetailByRegionIdAndDAte(int regionId, DateTime startDate, DateTime endDate);

        List<ShiftDetail> GetAllShiftDetailsFromShiftId(int shiftId);

        Task<bool> AddShift(Shift shift);

        Task<bool> AddShiftDetails(List<ShiftDetail> shiftDetails);

        Task<bool> AddShiftDetailsRegion(List<ShiftDetailRegion> shiftDetailRegions);

        Task<bool> UpdateShiftDetails(List<ShiftDetail> shiftDetails);

        Task<bool> UpdateShiftDetailRegions(List<ShiftDetailRegion> shiftDetailRegions);

        ShiftDetail GetShiftDetails(int shiftDetailsId);

        ShiftDetail GetShiftDetailsWithPhysician(int shiftDetailsId);

        ShiftDetailRegion GetShiftDetailRegion(int shiftDetailsId);

        List<ShiftDetail> GetAllShiftDetails(int regionId, bool isThisMonth, DateTime date, int skip);

        int CountAllShiftDetails(int regionId, bool isThisMonth, DateTime date);
    }
}