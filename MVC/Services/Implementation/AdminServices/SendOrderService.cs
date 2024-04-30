using Repositories.DataModels;
using Repositories.Interfaces;
using Services.Interfaces.AdminServices;
using Services.ViewModels.Admin;

namespace Services.Implementation.AdminServices
{
    public class SendOrderService : ISendOrderService
    {
        private readonly IHealthProfessionalRepository _healthProfessionalRepository;

        public SendOrderService(IHealthProfessionalRepository healthProfessionalRepository)
        {
            _healthProfessionalRepository = healthProfessionalRepository;
        }

        public SendOrder GetSendOrderDetails(int requestId)
        { 
            return new SendOrder()
            {
                Professions = _healthProfessionalRepository.GetHealthProfessionalTypes()
                                                    .ToDictionary(healthProfessionalType => healthProfessionalType.HealthProfessionalId,
                                                                  healthProfessionalType => healthProfessionalType.ProfessionName),
            };
        }

        public HealthProfessional GetBusinessData(int venderId)
        {
            return _healthProfessionalRepository.GetHealthProfessional(venderId);
        }

        public async Task<bool> AddOrderDetails(SendOrder model,int requestId)
        {
            return await _healthProfessionalRepository.AddOrderDetails(
                new OrderDetail()
                {
                    VendorId = model.SelectedBusiness,
                    RequestId = requestId,
                    FaxNumber = model.FaxNumber,
                    Email = model.Email,
                    BusinessContact = model.Contact,
                    Prescription = model.OrderDetails,
                    NoOfRefill = model.NoOfRefill,
                    CreatedDate = DateTime.Now,
                });
        }

        public Dictionary<int, string> GetBusinessByProfession(int professionId)
        {
            return _healthProfessionalRepository.GetHealthProfessionalByProfession(professionId)
                                           .ToDictionary(healthProfessional => healthProfessional.VendorId,
                                                         healthProfessional => healthProfessional.VendorName);
        }
    }
}
