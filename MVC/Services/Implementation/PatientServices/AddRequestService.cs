using Repositories.DataModels;
using Repositories.Interface;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Interfaces.PatientServices;
using Services.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace Services.Implementation.PatientServices
{
    public class AddRequestService : IAddRequestService
    {
        private readonly IAspRepository _aspRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IRequestWiseFileRepository _requestWiseFileRepository;
        private readonly IRequestClientRepository _requestClientRepository;
        private readonly IFileService _fileService;
        private readonly IBusinessConciergeRepository _businessConciergeRepository;

        public AddRequestService(IAspRepository aspRepository,
                                    IUserRepository userRepository, IRequestRepository requestRepository,
                                    IRequestWiseFileRepository requestWiseFileRepository, IBusinessConciergeRepository businessConciergeRepository,
                                    IRequestClientRepository requestClientRepository, IFileService fileService)
        {
            _aspRepository = aspRepository;
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _requestWiseFileRepository = requestWiseFileRepository;
            _requestClientRepository = requestClientRepository;
            _fileService = fileService;
            _businessConciergeRepository = businessConciergeRepository;
        }

        public bool IsEmailExists(String email)
        {
            int aspNetUserId = _aspRepository.CheckUser(email);
            return aspNetUserId == 0 ? false : true;
        }

        public Dictionary<int,String> GetRegions()
        {
            return _requestClientRepository.GetAllRegions().ToDictionary(region => region.RegionId, region => region.Name);
        }

        private async Task<int> CheckAspNetRole(String role)
        {
            int aspNetRoleId = _aspRepository.CheckUserRole(role);
            if (aspNetRoleId == 0)
            {
                AspNetRole aspNetRole = new()
                {
                    Name = role,
                };
                aspNetRoleId = await _aspRepository.AddUserRole(aspNetRole);
            }
            return aspNetRoleId;
        }

        public async Task<bool> AddPatientRequest(AddPatientRequest model)
        {
            int aspNetRoleId = await CheckAspNetRole(role: "Patient");
            int aspNetUserId = _aspRepository.CheckUser(email: model.Email);
            int userId = _userRepository.GetUserID(aspNetUserId);
            if (aspNetUserId == 0)
            {
                AspNetUser aspNetUser = new()
                {
                    UserName = model.FirstName,
                    Email = model.Email,
                    PhoneNumber = model.Mobile,
                    PasswordHash = GenrateHash(model.Password),
                    CreatedDate = DateTime.Now,
                };
                aspNetUserId = await _aspRepository.AddUser(aspNetUser);
                User user = new()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    Street = model.Street,
                    City = model.City,
                    State = model.State,
                    ZipCode = model.ZipCode,
                    AspNetUserId = aspNetUserId,
                    CreatedBy = aspNetUserId,
                    CreatedDate = DateTime.Now,
                    House = model.House,
                    IntYear = model.BirthDate.Year,
                    IntDate = model.BirthDate.Day,
                    StrMonth = model.BirthDate.Month.ToString(),
                };
                userId = await _userRepository.AddUser(user);
                AspNetUserRole aspNetUserRole = new()
                {
                    UserId = aspNetUserId,
                    RoleId = aspNetRoleId,
                };
                await _aspRepository.AddAspNetUserRole(aspNetUserRole);
                _fileService.SendNewAccountMail(model.Email, model.Password);
            }
            Request request = new()
            {
                RequestTypeId = 2,
                UserId = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.Mobile,
                CreatedDate = DateTime.Now,
            };
            int requestId = await _requestRepository.AddRequest(request);
            if (model.File != null)
            {
                await _fileService.AddFile(requestId: requestId, file: model.File, firstName: model.FirstName, lastName:model.LastName);
            }
            RequestClient requestClient = new()
            {
                RequestId = requestId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.Mobile,
                Email = model.Email,
                State = model.State,
                Street = model.Street,
                City = model.City,
                ZipCode = model.ZipCode,
                Status = 1,
                Symptoms = model.Symptoms,
                IntYear = DateTime.Now.Year,
                IntDate = DateTime.Now.Day,
                StrMonth = DateTime.Now.Month.ToString(),
                RegionId = model.Region,
            };
            return await _requestClientRepository.AddRequestClient(requestClient);
        }  

        public AddRequestByPatient GetModelForRequestByMe(int aspNetUserId)
        {
            User user = _userRepository.GetUser(aspNetUserId);
            DateTime birthDay = DateTime.Parse(user.IntYear + "-" + user.StrMonth + "-" + user.IntDate);
            AddRequestByPatient addRequestForMe = new()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = birthDay,
                Mobile = user.Mobile,
                Email = user.Email,
            };
            return addRequestForMe;
        }

        public async Task<bool> AddRequestForMe(AddRequestByPatient model)
        {
            AddPatientRequest patientRequest = new()
            {
                Symptoms = model.Symptoms,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Mobile = model.Mobile,
                Password = model.Password,
                Street = model.Street,
                City = model.City,
                State = model.State,
                ZipCode = model.ZipCode,
                House = model.House,
                BirthDate = model.BirthDate,
                File = model.File,
                Region = model.Region,
            };
            return await AddPatientRequest(patientRequest);
        }

        public async Task<bool> AddRequestForSomeOneelse(AddRequestByPatient model, int aspNetUserIdMe)
        {
            int aspNetUserId = _aspRepository.CheckUser(email: model.Email);
            int userId = _userRepository.GetUserID(aspNetUserId);
            if (aspNetUserId == 0)
            {
                AspNetUser aspNetUser = new()
                {
                    UserName = model.FirstName,
                    Email = model.Email,
                    PhoneNumber = model.Mobile,
                    PasswordHash = GenrateHash(model.Password),
                    CreatedDate = DateTime.Now,
                };
                aspNetUserId = await _aspRepository.AddUser(aspNetUser);
                User user = new()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    Street = model.Street,
                    City = model.City,
                    State = model.State,
                    ZipCode = model.ZipCode,
                    AspNetUserId = aspNetUserId,
                    CreatedBy = aspNetUserId,
                    CreatedDate = DateTime.Now,
                    House = model.House,
                    IntYear = model.BirthDate.Year,
                    IntDate = model.BirthDate.Day,
                    StrMonth = model.BirthDate.Month.ToString(),
                };
                userId = await _userRepository.AddUser(user);
                AspNetUserRole aspNetUserRole = new()
                {
                    UserId = aspNetUserId,
                    RoleId = _aspRepository.CheckUserRole(role: "Patient"),
                };
                await _aspRepository.AddAspNetUserRole(aspNetUserRole);
                _fileService.SendNewAccountMail(model.Email, model.Password);
            }
            User userMe = _userRepository.GetUser(aspNetUserIdMe);
            Request request = new()
            {
                RequestTypeId = 3,
                FirstName = userMe.FirstName,
                LastName = userMe.LastName,
                Email = userMe.Email,
                PhoneNumber = userMe.Mobile,
                UserId = userId,
                RelationName = model.Relation,
                CreatedDate = DateTime.Now,
            };
            int requestId = await _requestRepository.AddRequest(request);
            if (model.File != null)
            {
                await _fileService.AddFile(requestId: requestId, file: model.File, firstName: model.FirstName, lastName: model.LastName);
            }
            RequestClient requestClient = new()
            {
                RequestId = requestId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.Mobile,
                Email = model.Email,
                State = model.State,
                Street = model.Street,
                City = model.City,
                ZipCode = model.ZipCode,
                Status = 1,
                Symptoms = model.Symptoms,
                IntYear = DateTime.Now.Year,
                IntDate = DateTime.Now.Day,
                StrMonth = DateTime.Now.Month.ToString(),
                RegionId = model.Region,
            };
            return await _requestClientRepository.AddRequestClient(requestClient);
        }

        public async Task<bool> AddConciergeRequest(AddConciergeRequest model)
        {
            int aspNetRoleId = await CheckAspNetRole(role: "Patient");
            int aspNetUserId = _aspRepository.CheckUser(email: model.Email);
            int userId = _userRepository.GetUserID(aspNetUserId);
            if (aspNetUserId == 0)
            {
                AspNetUser aspNetUser = new()
                {
                    UserName = model.FirstName,
                    Email = model.Email,
                    PhoneNumber = model.Mobile,
                    PasswordHash = GenrateHash(password: model.Password),
                    CreatedDate = DateTime.Now,
                };
                aspNetUserId = await _aspRepository.AddUser(aspNetUser);
                User user = new()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    Street = model.Street,
                    City = model.City,
                    State = model.State,
                    ZipCode = model.ZipCode,
                    AspNetUserId = aspNetUserId,
                    CreatedBy = aspNetUserId,
                    CreatedDate = DateTime.Now,
                    House = model.House,
                    IntYear = model.BirthDate.Value.Year,
                    IntDate = model.BirthDate.Value.Day,
                    StrMonth = model.BirthDate.Value.Month.ToString(),
                };
                userId = await _userRepository.AddUser(user);
                AspNetUserRole aspNetUserRole = new()
                {
                    UserId = aspNetUserId,
                    RoleId = aspNetRoleId,
                };
                await _aspRepository.AddAspNetUserRole(aspNetUserRole);
                _fileService.SendNewAccountMail(model.Email, model.Password);
            }
            Request request = new()
            {
                RequestTypeId = 4,
                UserId = userId,
                FirstName = model.ConciergeFirstName,
                LastName = model.ConciergeLastName,
                Email = model.ConciergeEmail,
                PhoneNumber = model.ConciergeMobile,
                CreatedDate = DateTime.Now,
            };
            int requestId = await _requestRepository.AddRequest(request);
            if (model.File != null)
            {
                await _fileService.AddFile(requestId: requestId, file: model.File, firstName: model.ConciergeFirstName, lastName: model.ConciergeLastName);
            }
            Concierge concierge = new()
            {
                ConciergeName = model.ConciergeFirstName,
                Street = model.ConciergeStreet,
                City = model.ConciergeCity,
                State = model.ConciergeState,
                ZipCode = model.ConciergeZipCode,
                CreatedDate = DateTime.Now,
            };
            int conciergeId = await _businessConciergeRepository.AddConcierge(concierge);
            RequestConcierge requestConcierge = new()
            {
                RequestId = request.RequestId,
                ConciergeId = concierge.ConciergeId
            };
            await _businessConciergeRepository.AddRequestConcierge(requestConcierge);
            RequestClient requestClient = new()
            {
                RequestId = requestId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.Mobile,
                Email = model.Email,
                State = model.State,
                Street = model.Street,
                City = model.City,
                ZipCode = model.ZipCode,
                Status = 1,
                Symptoms = model.Symptoms,
                IntYear = DateTime.Now.Year,
                IntDate = DateTime.Now.Day,
                StrMonth = DateTime.Now.Month.ToString(),
                RegionId = model.Region,
            };
            return await _requestClientRepository.AddRequestClient(requestClient);
        }

        public async Task<bool> AddFamilyFriendRequest(AddFamilyRequest model)
        {
            int aspNetRoleId = await CheckAspNetRole(role: "Patient");
            int aspNetUserId = _aspRepository.CheckUser(email: model.Email);
            int userId = _userRepository.GetUserID(aspNetUserId);
            if (aspNetUserId == 0)
            {
                AspNetUser aspNetUser = new()
                {
                    UserName = model.FirstName,
                    Email = model.Email,
                    PhoneNumber = model.Mobile,
                    PasswordHash = GenrateHash(model.Password),
                    CreatedDate = DateTime.Now,
                };
                aspNetUserId = await _aspRepository.AddUser(aspNetUser);
                User user = new()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    Street = model.Street,
                    City = model.City,
                    State = model.State,
                    ZipCode = model.ZipCode,
                    AspNetUserId = aspNetUserId,
                    CreatedBy = aspNetUserId,
                    CreatedDate = DateTime.Now,
                    House = model.House,
                    IntYear = model.BirthDate.Value.Year,
                    IntDate = model.BirthDate.Value.Day,
                    StrMonth = model.BirthDate.Value.Month.ToString(),
                };
                userId = await _userRepository.AddUser(user);
                AspNetUserRole aspNetUserRole = new()
                {
                    UserId = aspNetUserId,
                    RoleId = aspNetRoleId,
                };
                await _aspRepository.AddAspNetUserRole(aspNetUserRole);
                _fileService.SendNewAccountMail(model.Email, model.Password);
            }
            Request request = new()
            {
                RequestTypeId = 3,
                UserId = userId,
                FirstName = model.FamilyFriendFirstName,
                LastName = model.FamilyFriendLastName,
                Email = model.FamilyFriendEmail,
                PhoneNumber = model.FamilyFriendMobile,
                CreatedDate = DateTime.Now,
                RelationName = model.Relation,
            };
            int requestId = await _requestRepository.AddRequest(request);
            if (model.File != null)
            {
                await _fileService.AddFile(requestId: requestId, file: model.File, firstName: model.FamilyFriendFirstName, 
                                                                                                      lastName: model.FamilyFriendLastName);
            }
            RequestClient requestClient = new()
            {
                RequestId = requestId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.Mobile,
                Email = model.Email,
                State = model.State,
                Street = model.Street,
                City = model.City,
                ZipCode = model.ZipCode,
                Status = 1,
                Symptoms = model.Symptoms,
                IntYear = DateTime.Now.Year,
                IntDate = DateTime.Now.Day,
                StrMonth = DateTime.Now.Month.ToString(),
                RegionId = model.Region,
            };
            return await _requestClientRepository.AddRequestClient(requestClient);
        }

        public async Task<bool> AddBusinessRequest(AddBusinessRequest model)
        {
            int aspNetRoleId = await CheckAspNetRole(role: "Patient");
            int aspNetUserId = _aspRepository.CheckUser(email: model.Email);
            int userId = _userRepository.GetUserID(aspNetUserId);
            if (aspNetUserId == 0)
            {
                AspNetUser aspNetUser = new()
                {
                    UserName = model.FirstName,
                    Email = model.Email,
                    PhoneNumber = model.Mobile,
                    PasswordHash = GenrateHash(model.Password),
                    CreatedDate = DateTime.Now,
                };
                aspNetUserId = await _aspRepository.AddUser(aspNetUser);
                User user = new()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    Street = model.Street,
                    City = model.City,
                    State = model.State,
                    ZipCode = model.ZipCode,
                    AspNetUserId = aspNetUserId,
                    CreatedBy = aspNetUserId,
                    CreatedDate = DateTime.Now,
                    House = model.House,
                    IntYear = model.BirthDate.Value.Year,
                    IntDate = model.BirthDate.Value.Day,
                    StrMonth = model.BirthDate.Value.Month.ToString(),
                };
                userId = await _userRepository.AddUser(user);
                AspNetUserRole aspNetUserRole = new()
                {
                    UserId = aspNetUserId,
                    RoleId = aspNetRoleId,
                };
                await _aspRepository.AddAspNetUserRole(aspNetUserRole);
                _fileService.SendNewAccountMail(model.Email, model.Password);
            }
            Request request = new()
            {
                RequestTypeId = 1,
                UserId = userId,
                FirstName = model.BusinessFirstName,
                LastName = model.BusinessLastName,
                Email = model.BusinessEmail,
                PhoneNumber = model.BusinessMobile,
                CreatedDate = DateTime.Now,
            };
            int requestId = await _requestRepository.AddRequest(request);
            if (model.File != null)
            {
                await _fileService.AddFile(requestId: requestId, file: model.File, firstName: model.BusinessFirstName, lastName: model.BusinessLastName);
            }
            Business business = new()
            {
                Name = model.BusinessFirstName,
                PhoneNumber = model.BusinessMobile,
                CreatedDate = DateTime.Now,
            };
            int businessId = await _businessConciergeRepository.AddBusiness(business);
            RequestBusiness requestBusiness = new()
            {
                RequestId = request.RequestId,
                BusinessId = business.BusinessId,
            };
            await _businessConciergeRepository.AddRequestBusiness(requestBusiness);
            RequestClient requestClient = new()
            {
                RequestId = requestId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.Mobile,
                Email = model.Email,
                State = model.State,
                Street = model.Street,
                City = model.City,
                ZipCode = model.ZipCode,
                Status = 1,
                Symptoms = model.Symptoms,
                IntYear = DateTime.Now.Year,
                IntDate = DateTime.Now.Day,
                StrMonth = DateTime.Now.Month.ToString(),
                RegionId = model.Region,
            };
            return await _requestClientRepository.AddRequestClient(requestClient);
        }

        private String GenrateHash(String password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(password.Trim()));
                return BitConverter.ToString(hashPassword).Replace("-", "").ToLower();
            }
        }
    }
}
