using Repositories.DataModels;
using Repositories.Interfaces;
using Services.Interfaces.AdminServices;
using Services.ViewModels.Admin;
using System.Collections;

namespace Services.Implementation.AdminServices
{
    public class PartnersService : IPartnersService
    {
        private readonly IHealthProfessionalRepository _healthProfessionalRepository;
        private readonly IRequestClientRepository _requestClientRepository;

        public PartnersService(IHealthProfessionalRepository healthProfessionalRepository,IRequestClientRepository requestClientRepository)
        {
            _healthProfessionalRepository = healthProfessionalRepository;
            _requestClientRepository = requestClientRepository;
        }

        public Partners GetPartnersData()
        {
            List<PartnersTableData> partnersTableDatas = _healthProfessionalRepository.GetHealthProfessionalByProfessionWithType(0,null)
                .Select(healthProfessional => new PartnersTableData()
                {
                    VenderId = healthProfessional.VendorId,
                    Profession = healthProfessional.ProfessionNavigation.ProfessionName,
                    BusinessName = healthProfessional.VendorName,
                    BusinessContact = healthProfessional.BusinessContact,
                    Email = healthProfessional.Email,
                    FaxNumber = healthProfessional.FaxNumber,
                    PhoneNumber = healthProfessional.PhoneNumber,
                }).ToList();
            return new Partners()
            {
                PartnersTableDatas = partnersTableDatas,
                ProfessionList = _healthProfessionalRepository.GetHealthProfessionalTypes()
                                       .ToDictionary(healthProfessionalType => healthProfessionalType.HealthProfessionalId,
                                                     healthProfessionalType => healthProfessionalType.ProfessionName),
            };
        }

        public List<PartnersTableData> GetPartnersTableDatas(int regionId, String searchElement)
        {
            return _healthProfessionalRepository.GetHealthProfessionalByProfessionWithType(regionId,searchElement)
                .Select(healthProfessional => new PartnersTableData()
                {
                    VenderId = healthProfessional.VendorId,
                    Profession = healthProfessional.ProfessionNavigation.ProfessionName,
                    BusinessName = healthProfessional.VendorName,
                    BusinessContact = healthProfessional.BusinessContact,
                    Email = healthProfessional.Email,
                    FaxNumber = healthProfessional.FaxNumber,
                    PhoneNumber = healthProfessional.PhoneNumber,
                }).ToList();
        }

        public BusinessProfile AddBusiness(bool isUpdate, int venderId)
        {
            if (isUpdate)
            {
                HealthProfessional healthProfessional = _healthProfessionalRepository.GetHealthProfessional(venderId);
                return new BusinessProfile()
                {
                    VendorName = healthProfessional.VendorName,
                    Profession = healthProfessional.Profession,
                    FaxNumber = healthProfessional.FaxNumber,
                    PhoneNumber = healthProfessional.PhoneNumber,
                    Email = healthProfessional.Email,
                    Street = healthProfessional.Address,
                    City = healthProfessional.City,
                    Zip = healthProfessional.Zip,
                    State = healthProfessional.RegionId.ToString(),
                    BusinessContact = healthProfessional.BusinessContact,
                    AspAction = "UpdateBusiness",
                    IsUpdate = isUpdate,
                    RegionList = _requestClientRepository.GetAllRegions().ToDictionary(region => region.RegionId, region => region.Name),
                    ProfessionList = _healthProfessionalRepository.GetHealthProfessionalTypes()
                                            .ToDictionary(healthProfessionalType => healthProfessionalType.HealthProfessionalId,
                                                          healthProfessionalType => healthProfessionalType.ProfessionName),
                };
            }
            else
            {
                return new BusinessProfile()
                {
                    AspAction = "CreateBusiness",
                    IsUpdate = isUpdate,
                    RegionList = _requestClientRepository.GetAllRegions().ToDictionary(region => region.RegionId, region => region.Name),
                    ProfessionList = _healthProfessionalRepository.GetHealthProfessionalTypes()
                                            .ToDictionary(healthProfessionalType => healthProfessionalType.HealthProfessionalId,
                                                          healthProfessionalType => healthProfessionalType.ProfessionName),
                };
            }
        }

        public async Task<bool> CreateBusiness(BusinessProfile businessProfile)
        {
            HealthProfessional healthProfessional = new HealthProfessional()
            {
                VendorName = businessProfile.VendorName,
                Profession = businessProfile.Profession,
                FaxNumber = businessProfile.FaxNumber,
                PhoneNumber = businessProfile.PhoneNumber,
                Email = businessProfile.Email,
                City = businessProfile.City,
                Zip = businessProfile.Zip,
                RegionId =int.Parse(businessProfile.State),
                CreatedDate = DateTime.Now,
                BusinessContact = businessProfile.BusinessContact,
                IsDeleted = new BitArray(1, false),
        };
            return await _healthProfessionalRepository.AddHealthProfessional(healthProfessional);
        }

        public async Task<bool> EditBusiness(BusinessProfile businessProfile, int venderId)
        {
            HealthProfessional healthProfessional = _healthProfessionalRepository.GetHealthProfessional(venderId);
            healthProfessional.Profession = businessProfile.Profession;
            healthProfessional.VendorName = businessProfile.VendorName;
            healthProfessional.FaxNumber = businessProfile.FaxNumber;
            healthProfessional.PhoneNumber = businessProfile.PhoneNumber;
            healthProfessional.Email = businessProfile.Email;
            healthProfessional.City = businessProfile.City;
            healthProfessional.Zip = businessProfile.Zip;
            healthProfessional.RegionId = int.Parse(businessProfile.State);
            healthProfessional.BusinessContact = businessProfile.BusinessContact;
            healthProfessional.ModifiedDate = DateTime.Now;
            return await _healthProfessionalRepository.UpdateHealthProfessional(healthProfessional);
        }

        public async Task<bool> DeleteBusiness(int venderId)
        {
            HealthProfessional healthProfessional = _healthProfessionalRepository.GetHealthProfessional(venderId);
            healthProfessional.IsDeleted = new BitArray(1, true);
            healthProfessional.ModifiedDate = DateTime.Now;
            return await _healthProfessionalRepository.UpdateHealthProfessional(healthProfessional);
        }
    }
}
