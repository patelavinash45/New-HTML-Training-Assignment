using Services.ViewModels.Admin;

namespace Services.Interfaces.AdminServices
{
    public interface IPartnersService
    {
        Partners GetPartnersData();

        List<PartnersTableData> GetPartnersTableDatas(int regionId, String searchElement);

        BusinessProfile AddBusiness(bool isUpdate, int venderId);

        Task<bool> CreateBusiness(BusinessProfile businessProfile);

        Task<bool> EditBusiness(BusinessProfile businessProfile, int venderId);

        Task<bool> DeleteBusiness(int venderId);
    }
}
