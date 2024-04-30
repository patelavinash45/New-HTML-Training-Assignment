using Repositories.DataModels;
using Services.ViewModels.Admin;

namespace Services.Interfaces.AdminServices
{
    public interface ISendOrderService
    {
        SendOrder GetSendOrderDetails(int requestId);

        HealthProfessional GetBusinessData(int venderId);

        Task<bool> AddOrderDetails(SendOrder model, int requestId); 

        Dictionary<int, string> GetBusinessByProfession(int professionId);
    }
}
