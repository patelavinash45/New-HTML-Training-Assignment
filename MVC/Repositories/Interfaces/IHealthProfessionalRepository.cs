using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IHealthProfessionalRepository
    {
        Task<bool> AddHealthProfessional(HealthProfessional healthProfessional);

        Task<bool> UpdateHealthProfessional(HealthProfessional healthProfessional);

        List<HealthProfessionalType> GetHealthProfessionalTypes();

        List<HealthProfessional> GetHealthProfessionalByProfessionWithType(int professionId, String searchElement);

        List<HealthProfessional> GetHealthProfessionalByProfession(int professionId);

        HealthProfessional GetHealthProfessional(int VenderId);

        Task<bool> AddOrderDetails(OrderDetail orderDetail);
    }
}
