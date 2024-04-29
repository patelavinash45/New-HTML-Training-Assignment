using Microsoft.AspNetCore.Http;
using Repositories.DataModels;
using Repositories.Interface;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Interfaces.AdminServices;
using Services.ViewModels.Admin;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Services.Implementation.AdminServices
{
    public class ProvidersService : IProvidersService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestClientRepository _requestClientRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAspRepository _aspRepository;
        private readonly IShiftRepository _shiftRepository;
        private readonly ILogsService _logsService;
        private readonly IFileService _fileService;

        public ProvidersService(IUserRepository userRepository, IRequestClientRepository requestClientRepository, IRoleRepository roleRepository,
                                 IAspRepository aspRepository, IShiftRepository shiftRepository, ILogsService logsService, IFileService fileService)
        {
            _userRepository = userRepository;
            _requestClientRepository = requestClientRepository;
            _roleRepository = roleRepository;
            _aspRepository = aspRepository;
            _shiftRepository = shiftRepository;
            _logsService = logsService;
            _fileService = fileService;
        }

        public Task<bool> EditShiftDetails(string data, int aspNetUserId)
        {
            ViewShift viewShift = JsonSerializer.Deserialize<ViewShift>(data);
            ShiftDetail shiftDetail = _shiftRepository.GetShiftDetails(int.Parse(viewShift.ShiftDetailsId));
            shiftDetail.ShiftDate = DateTime.Parse(viewShift.ShiftDate.ToString());
            shiftDetail.StartTime = viewShift.StartTime;
            shiftDetail.EndTime = viewShift.EndTime;
            shiftDetail.ModifiedBy = aspNetUserId;
            shiftDetail.ModifiedDate = DateTime.Now;
            return _shiftRepository.UpdateShiftDetails(new List<ShiftDetail> { shiftDetail });
        }

        public ViewShift GetShiftDetails(int shiftDetailsId)
        {
            ShiftDetail shiftDetail = _shiftRepository.GetShiftDetailsWithPhysician(shiftDetailsId);
            return new ViewShift()
            {
                ShiftDetailsId = shiftDetailsId.ToString(),
                PhysicianName = $"{shiftDetail.Shift.Physician.FirstName} {shiftDetail.Shift.Physician.LastName}",
                Region = shiftDetail.Region.Name,
                ShiftDate = DateOnly.FromDateTime(shiftDetail.ShiftDate),
                StartTime = shiftDetail.StartTime,
                EndTime = shiftDetail.EndTime,
            };
        }

        public List<ProviderLocation> GetProviderLocation()
        {
            return _userRepository.GetAllProviderLocation()
                .Select(physicianLocation => new ProviderLocation
                {
                    ProviderName = physicianLocation.PhysicianName,
                    latitude = physicianLocation.Latitude,
                    longitude = physicianLocation.Longitude,
                }).ToList();
        }

        public Provider GetProviders(int regionId)
        {
            Dictionary<int, string> regions = new Dictionary<int, string>();
            if (regionId == 0)  // for first time page load - on filter this part not execute
            {
                regions = _requestClientRepository.GetAllRegions().ToDictionary(region => region.RegionId, region => region.Name);
            }
            List<ProviderTable> providerTables = _userRepository.GetAllPhysiciansByRegionId(regionId)
                .Select(physician => new ProviderTable()
                {
                    FirstName = physician.FirstName,
                    LastName = physician.LastName,
                    Notification = physician.PhysicianNotifications.FirstOrDefault().IsNotificationStopped[0],
                    ProviderId = physician.PhysicianId,
                    Status = physician.Status == 1 ? "Active" : "Pending",
                }).ToList();
            return new Provider()
            {
                Providers = providerTables,
                Regions = regions,
            };
        }

        public ProviderOnCall GetProviderOnCall(int regionId)
        {
            return new ProviderOnCall()
            {
                Regions = _requestClientRepository.GetAllRegions().ToDictionary(region => region.RegionId, region => region.Name),
                ProviderList = GetProviderList(regionId),
            };
        }

        public ProviderList GetProviderList(int regionId)
        {
            string path = "/Files//Providers/Photo/";
            List<ProviderOnCallTable> providerOnCalls = new List<ProviderOnCallTable>();
            List<ProviderOnCallTable> providerOffDuty = new List<ProviderOnCallTable>();
            _userRepository.GetAllPhysicianWithCurrentShift(regionId)
                .ForEach(physician =>
                {
                    foreach (var shift in physician.Shifts.Where(x => x.ShiftDetails.Count > 0))
                    {
                        providerOnCalls.Add(new ProviderOnCallTable()
                        {
                            Photo = $"{path}{physician.AspNetUserId}/{physician.Photo}",
                            FirstName = physician.FirstName,
                            LastName = physician.LastName,
                        });
                        goto NextPhysician;
                    }
                    providerOffDuty.Add(new ProviderOnCallTable()
                    {
                        Photo = $"{path}{physician.AspNetUserId}/{physician.Photo}",
                        FirstName = physician.FirstName,
                        LastName = physician.LastName,
                    });
                NextPhysician:;
                });
            return new ProviderList()
            {
                ProviderOffDuty = providerOffDuty,
                ProviderOnCall = providerOnCalls,
            };
        }

        public async Task<bool> EditProviderNotification(int providerId, bool isNotification)
        {
            PhysicianNotification physicianNotification = _userRepository.GetPhysicianNotification(providerId);
            physicianNotification.IsNotificationStopped[0] = isNotification;
            return await _userRepository.UpdatePhysicianNotification(physicianNotification);
        }

        public async Task<bool> SaveSign(string sign, int physicianId)
        {
            String path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Providers/Sign/" + physicianId.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            };
            path = Path.Combine(path, "Sign.png");
            byte[] bytes = Convert.FromBase64String(sign.Split(",")[1]);
            File.WriteAllBytes(path, bytes);
            Physician physician = _userRepository.GetPhysicianWithAspNetUser(physicianId);
            physician.IsSignature = new BitArray(1, true);
            return await _userRepository.UpdatePhysician(physician);
        }

        public async Task<bool> ContactProvider(ContactProvider model)
        {
            if (model.email)
            {
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("tatva.dotnet.avinashpatel@outlook.com"),
                    Subject = "Message From Admin",
                    IsBodyHtml = true,
                    Body = model.Message,
                };
                Physician physician = _userRepository.GetPhysicianByPhysicianId(model.providerId);
                //mailMessage.To.Add(physician.Email);
                mailMessage.To.Add("tatva.dotnet.avinashpatel@outlook.com");
                SmtpClient smtpClient = new SmtpClient("smtp.office365.com")
                {
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true,
                    Port = 587,
                    Credentials = new NetworkCredential(userName: "tatva.dotnet.avinashpatel@outlook.com", password: "Avinash@6351"),
                };
                try
                {
                    smtpClient.SendMailAsync(mailMessage);
                    EmailLog emailLog = new EmailLog()
                    {
                        Name = $"{physician.FirstName} {physician.LastName}",
                        SubjectName = "Message From Admin to Provider",
                        EmailId = physician.Email,
                        CreateDate = DateTime.Now,
                        SentDate = DateTime.Now,
                        IsEmailSent = new BitArray(1, true),
                        RoleId = physician.RoleId,
                    };
                    await _logsService.AddEmailLog(emailLog);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }

        public CreateProvider GetCreateProvider()
        {
            return new CreateProvider()
            {
                Regions = _requestClientRepository.GetAllRegions().ToDictionary(region => region.RegionId, region => region.Name),
                Roles = _roleRepository.GetRolesByUserType(3).ToDictionary(role => role.RoleId, role => role.Name),
            };
        }

        public async Task<String> CreateProvider(CreateProvider model)
        {
            if (_aspRepository.CheckUser(model.Email) == 0)
            {
                int aspNetRoleId = _aspRepository.CheckUserRole(role: "Physician");
                if (aspNetRoleId == 0)
                {
                    AspNetRole aspNetRole = new()
                    {
                        Name = "Physician",
                    };
                    aspNetRoleId = await _aspRepository.AddUserRole(aspNetRole);
                }
                AspNetUser aspNetUser = new()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    PasswordHash = GenrateHash(model.Password),
                    CreatedDate = DateTime.Now,
                };
                int aspNetUserId = await _aspRepository.AddUser(aspNetUser);
                AspNetUserRole aspNetUserRole = new()
                {
                    UserId = aspNetUserId,
                    RoleId = aspNetRoleId,
                };
                await _aspRepository.AddAspNetUserRole(aspNetUserRole);
                FilePickUp("Photo", aspNetUserId, model.Photo);
                if (model.IsAgreementDoc)
                {
                    FilePickUp("AgreementDoc", aspNetUserId, model.AgreementDoc);
                }
                if (model.IsBackgroundDoc)
                {
                    FilePickUp("BackgroundDoc", aspNetUserId, model.BackgroundDoc);
                }
                if (model.IsHIPAACompliance)
                {
                    FilePickUp("HIPAACompliance", aspNetUserId, model.HIPAACompliance);
                }
                if (model.IsNonDisclosureDoc)
                {
                    FilePickUp("NonDisclosureDoc", aspNetUserId, model.NonDisclosureDoc);
                }
                Physician physician = new Physician()
                {
                    AspNetUserId = aspNetUserId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Mobile = model.Phone,
                    RoleId = model.SelectedRole,
                    MedicalLicense = model.MedicalLicance,
                    Address1 = model.Add1,
                    Address2 = model.Add2,
                    City = model.City,
                    Zip = model.Zip,
                    RegionId = int.Parse(model.SelectedRegion),
                    AltPhone = model.Phone2,
                    CreatedDate = DateTime.Now,
                    Status = 0,
                    BusinessName = model.BusinessName,
                    BusinessWebsite = model.BusinessWebsite,
                    Npinumber = model.NpiNumber,
                    IsAgreementDoc = new BitArray(1, model.IsAgreementDoc),
                    IsBackgroundDoc = new BitArray(1, model.IsBackgroundDoc),
                    IsNonDisclosureDoc = new BitArray(1, model.IsNonDisclosureDoc),
                    IsTrainingDoc = new BitArray(1, model.IsHIPAACompliance),
                    IsSignature = new BitArray(1, false),
                    Photo = model.Photo.FileName,
                    AdminNotes = model.AdminNotes,
                };
                if (await _userRepository.AddPhysician(physician))
                {
                    PhysicianNotification physicianNotification = new PhysicianNotification()
                    {
                        PhysicianId = physician.PhysicianId,
                        IsNotificationStopped = new BitArray(1, false)
                    };
                    if (await _userRepository.AddPhysicianNotification(physicianNotification))
                    {
                        List<PhysicianRegion> physicianRegions = new List<PhysicianRegion>();
                        foreach (String regionId in model.SelectedRegions)
                        {
                            PhysicianRegion physicianRegion = new PhysicianRegion()
                            {
                                PhysicianId = physician.PhysicianId,
                                RegionId = int.Parse(regionId),
                            };
                            physicianRegions.Add(physicianRegion);
                        }
                        if (await _userRepository.AddPhysicianRegions(physicianRegions))
                        {
                            physician.IsNotification = physicianNotification.Id;
                            _fileService.SendNewAccountMail(model.Email, model.Password);
                            return await _userRepository.UpdatePhysician(physician) ? "" : "Failed !!";
                        }
                    }
                }
                return "Failed !!";
            }
            return "Email Already Exits";
        }

        public EditProvider GetEditProvider(int physicianId)
        {
            Physician physician = _userRepository.GetPhysicianWithAspNetUser(physicianId);
            EditProvider editProvider = new EditProvider()
            {
                UserName = physician.AspNetUser.UserName,
                FirstName = physician.FirstName,
                LastName = physician.LastName,
                Add1 = physician.Address1,
                Add2 = physician.Address2,
                SelectedRole = physician.RoleId,
                Status = physician.Status,
                Email = physician.Email,
                Phone = physician.Mobile,
                Phone2 = physician.AltPhone,
                MedicalLicance = physician.MedicalLicense,
                SynchronizationEmail = physician.SyncEmailAddress != null ? physician.SyncEmailAddress : "",
                NpiNumber = physician.Npinumber,
                City = physician.City,
                Zip = physician.Zip,
                SelectedRegion = physician.RegionId,
                BusinessName = physician.BusinessName,
                BusinessWebsite = physician.BusinessWebsite,
                IsSignature = physician.IsSignature[0],
                SignaturePath = physician.IsSignature[0] ? GetFile("Sign", (int)physician.PhysicianId) : null,
                AdminNotes = physician.AdminNotes != null ? physician.AdminNotes : "",
                AgreementDocPath = physician.IsAgreementDoc[0] ? GetFile("AgreementDoc", (int)physician.AspNetUserId) : null,
                IsAgreementDoc = physician.IsAgreementDoc[0],
                BackgroundDocPath = physician.IsBackgroundDoc[0] ? GetFile("BackgroundDoc", (int)physician.AspNetUserId) : null,
                IsBackgroundDoc = physician.IsBackgroundDoc[0],
                HIPAACompliancePath = physician.IsTrainingDoc[0] ? GetFile("HIPAACompliance", (int)physician.AspNetUserId) : null,
                IsHIPAACompliance = physician.IsTrainingDoc[0],
                NonDisclosureDocPath = physician.IsNonDisclosureDoc[0] ? GetFile("NonDisclosureDoc", (int)physician.AspNetUserId) : null,
                IsNonDisclosureDoc = physician.IsNonDisclosureDoc[0],
                SelectedRegions = _userRepository.GetAllPhysicianRegionsByPhysicianId(physicianId).Select(x => x.RegionId).ToList(),
                Regions = _requestClientRepository.GetAllRegions().ToDictionary(region => region.RegionId, region => region.Name),
                Roles = _roleRepository.GetRolesByUserType(3).ToDictionary(role => role.RoleId, role => role.Name),
            };
            return editProvider;
        }

        public async Task<bool> EditphysicianAccountInformaction(EditProvider model, int physicianId, int aspNetUserId)
        {
            Physician physician = _userRepository.GetPhysicianWithAspNetUser(physicianId);
            physician.Status = (short)model.Status != null ? (short)model.Status : physician.Status;
            physician.RoleId = model.SelectedRole != null ? model.SelectedRole : physician.RoleId;
            physician.ModifiedBy = aspNetUserId;
            physician.ModifiedDate = DateTime.Now;
            if (await _userRepository.UpdatePhysician(physician))
            {
                physician.AspNetUser.PasswordHash = GenrateHash(model.Password);
                physician.AspNetUser.UserName = model.UserName;
                return await _aspRepository.ChangePassword(physician.AspNetUser);
            }
            return false;
        }

        public async Task<bool> EditphysicianPhysicianInformaction(EditProvider model, int physicianId, int aspNetUserId)
        {
            Physician physician = _userRepository.GetPhysicianByPhysicianId(physicianId);
            physician.FirstName = model.FirstName;
            physician.LastName = model.LastName;
            physician.Email = model.Email;
            physician.Mobile = model.Phone;
            physician.MedicalLicense = model.MedicalLicance;
            physician.Npinumber = model.NpiNumber;
            physician.SyncEmailAddress = model.SynchronizationEmail;
            physician.ModifiedBy = aspNetUserId;
            physician.ModifiedDate = DateTime.Now;
            if (await _userRepository.UpdatePhysician(physician))
            {
                List<PhysicianRegion> physicianRegions = _userRepository.GetAllPhysicianRegionsByPhysicianId(physicianId);
                List<PhysicianRegion> physicianRegionsDelete = new List<PhysicianRegion>();
                List<PhysicianRegion> physicianRegionsCreate = new List<PhysicianRegion>();
                foreach (PhysicianRegion physicianRegion in physicianRegions)
                {
                    if (!model.SelectedRegions.Contains(physicianRegion.RegionId))
                    {
                        physicianRegionsDelete.Add(physicianRegion);
                    }
                }
                foreach (int regionId in model.SelectedRegions)
                {
                    if (!physicianRegions.Any(a => a.RegionId == regionId))
                    {
                        physicianRegionsCreate.Add(
                            new PhysicianRegion()
                            {
                                PhysicianId = physicianId,
                                RegionId = regionId,
                            });
                    }
                }
                if (physicianRegionsCreate.Count > 0)
                {
                    await _userRepository.AddPhysicianRegions(physicianRegionsCreate);
                }
                if (physicianRegionsDelete.Count > 0)
                {
                    return await _userRepository.DeletePhysicianRegions(physicianRegionsDelete);
                }
                return true;
            }
            return false;
        }

        public async Task<bool> EditphysicianMailAndBillingInformaction(EditProvider model, int physicianId, int aspNetUserId)
        {
            Physician physician = _userRepository.GetPhysicianByPhysicianId(physicianId);
            physician.Address1 = model.Add1;
            physician.Address2 = model.Add2;
            physician.City = model.City;
            physician.RegionId = model.SelectedRegion;
            physician.Zip = model.Zip;
            physician.AltPhone = model.Phone2;
            physician.ModifiedBy = aspNetUserId;
            physician.ModifiedDate = DateTime.Now;
            return await _userRepository.UpdatePhysician(physician);
        }

        public async Task<bool> EditphysicianProviderProfile(EditProvider model, int physicianId, int aspNetUserId)
        {
            Physician physician = _userRepository.GetPhysicianByPhysicianId(physicianId);
            physician.BusinessName = model.BusinessName;
            physician.BusinessWebsite = model.BusinessWebsite;
            physician.Photo = model.Photo.FileName;
            physician.AdminNotes = model.AdminNotes;
            physician.ModifiedBy = aspNetUserId;
            physician.ModifiedDate = DateTime.Now;
            FilePickUp("Photo", (int)physician.AspNetUserId, model.Photo);
            if (model.Signature != null)
            {
                physician.IsSignature = new BitArray(1, true);
                FilePickUp("Sign", physician.PhysicianId, model.Signature);
            }
            return await _userRepository.UpdatePhysician(physician);
        }

        public async Task<bool> EditphysicianOnbordingInformaction(EditProvider model, int physicianId, int aspNetUserId)
        {
            Physician physician = _userRepository.GetPhysicianByPhysicianId(physicianId);
            if (model.IsAgreementDoc)
            {
                FilePickUp("AgreementDoc", aspNetUserId, model.AgreementDoc);
            }
            if (model.IsBackgroundDoc)
            {
                FilePickUp("BackgroundDoc", aspNetUserId, model.BackgroundDoc);
            }
            if (model.IsHIPAACompliance)
            {
                FilePickUp("HIPAACompliance", aspNetUserId, model.HIPAACompliance);
            }
            if (model.IsNonDisclosureDoc)
            {
                FilePickUp("NonDisclosureDoc", aspNetUserId, model.NonDisclosureDoc);
            }
            physician.IsAgreementDoc = new BitArray(1, model.IsAgreementDoc);
            physician.IsBackgroundDoc = new BitArray(1, model.IsBackgroundDoc);
            physician.IsNonDisclosureDoc = new BitArray(1, model.IsNonDisclosureDoc);
            physician.IsTrainingDoc = new BitArray(1, model.IsHIPAACompliance);
            physician.ModifiedBy = aspNetUserId;
            physician.ModifiedDate = DateTime.Now;
            return await _userRepository.UpdatePhysician(physician);
        }

        public ProviderScheduling GetProviderSchedulingData()
        {
            CreateShift createShift = new CreateShift()
            {
                Regions = _requestClientRepository.GetAllRegions().ToDictionary(region => region.RegionId, region => region.Name),
            };
            return new ProviderScheduling()
            {
                TableData = DayWiseScheduling(DateTime.Now, 0),
                CreateShift = createShift,
            };
        }

        public async Task<bool> CreateShift(CreateShift model, int aspNetUserId, bool isAdmin)
        {
            if (!isAdmin)
            {
                model.SelectedPhysician = _userRepository.GetPhysicianByAspNetUserId(aspNetUserId).PhysicianId;
            }
            DateTime date = (DateTime)model.ShiftDate;
            Shift shift = new Shift()
            {
                PhysicianId = model.SelectedPhysician,
                StartDate = new DateOnly(date.Year, date.Month, date.Day),
                IsRepeat = new BitArray(1, model.IsRepeat),
                WeekDays = model.IsRepeat ? String.Join("", model.SelectedDays) : "",
                RepeatUpto = model.RepeatEnd,
                CreatedBy = aspNetUserId,
                CreatedDate = DateTime.Now,
            };
            if (await _shiftRepository.AddShift(shift))
            {
                List<ShiftDetail> shiftDetails = new List<ShiftDetail>();
                ShiftDetail shiftDetail = new ShiftDetail()
                {
                    ShiftId = shift.ShiftId,
                    ShiftDate = (DateTime)model.ShiftDate,
                    RegionId = model.SelectedRegion,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    Status = 0,
                    IsDeleted = new BitArray(1, false)
                };
                shiftDetails.Add(shiftDetail);
                if (model.IsRepeat)
                {
                    foreach (int day in model.SelectedDays)
                    {
                        date = (DateTime)model.ShiftDate;
                        for (int i = 0; i < model.RepeatEnd; i++)
                        {
                            date = date.AddDays(day - (int)date.DayOfWeek + 7);
                            shiftDetail = new ShiftDetail()
                            {
                                ShiftId = shift.ShiftId,
                                ShiftDate = date,
                                RegionId = model.SelectedRegion,
                                StartTime = model.StartTime,
                                EndTime = model.EndTime,
                                Status = 0,
                                IsDeleted = new BitArray(1, false)
                            };
                            shiftDetails.Add(shiftDetail);
                        }
                    };
                }
                if (await _shiftRepository.AddShiftDetails(shiftDetails))
                {
                    return await _shiftRepository.AddShiftDetailsRegion(
                                    _shiftRepository.GetAllShiftDetailsFromShiftId(shift.ShiftId)
                                        .Select(shiftDetail => new ShiftDetailRegion()
                                        {
                                            ShiftDetailId = shiftDetail.ShiftDetailId,
                                            RegionId = model.SelectedRegion,
                                            IsDeleted = new BitArray(1, false),
                                        }).ToList());
                }
            }
            return false;
        }

        public RequestedShift GetRequestedShift()
        {
            return new RequestedShift()
            {
                Regions = _requestClientRepository.GetAllRegions().ToDictionary(region => region.RegionId, region => region.Name),
                RequestedShiftModel = GetRequestShiftTableDate(0, false, 1),
            };
        }

        public RequestShiftModel GetRequestShiftTableDate(int regionId, bool isMonth, int pageNo)
        {
            int skip = (pageNo - 1) * 10;
            DateTime date = new DateTime();
            if (isMonth)
            {
                date = DateTime.Now;
                date = new DateTime(date.Year, date.Month, 1);
            }
            int totalShifts = _shiftRepository.CountAllShiftDetails(regionId, isMonth, date);
            List<RequestedShiftTable> requestedShiftTables = _shiftRepository.GetAllShiftDetails(regionId, isMonth, date, skip)
                .Select(shiftDetails => new RequestedShiftTable
                {
                    Name = shiftDetails.Shift.Physician.FirstName + " " + shiftDetails.Shift.Physician.LastName,
                    Date = shiftDetails.ShiftDate,
                    StartTime = shiftDetails.StartTime,
                    EndTime = shiftDetails.EndTime,
                    Region = shiftDetails.Region.Name,
                    ShiftDetailsId = shiftDetails.ShiftDetailId,
                }).ToList();
            int totalPages = totalShifts % 10 != 0 ? (totalShifts / 10) + 1 : totalShifts / 10;
            return new RequestShiftModel()
            {
                TotalShifts = totalShifts,
                IsFirstPage = pageNo != 1,
                IsLastPage = pageNo != totalPages,
                IsNextPage = pageNo < totalPages,
                IsPreviousPage = pageNo > 1,
                PageNo = pageNo,
                StartRange = skip + 1,
                EndRange = skip + 10 < totalShifts ? skip + 10 : totalShifts,
                RequestedShiftTables = requestedShiftTables,
            };
        }

        public async Task<bool> ChangeShiftDetails(string dataList, bool isApprove, int aspNetUserId)
        {
            List<int> ids = JsonSerializer.Deserialize<List<String>>(dataList).Select(id => int.Parse(id)).ToList();
            if (isApprove)
            {
                List<ShiftDetail> shiftDetails = new List<ShiftDetail>();
                foreach (int id in ids)
                {
                    ShiftDetail shiftDetail = _shiftRepository.GetShiftDetails(id);
                    shiftDetail.Status = 1;
                    shiftDetail.ModifiedBy = aspNetUserId;
                    shiftDetail.ModifiedDate = DateTime.Now;
                    shiftDetails.Add(shiftDetail);
                }
                return await _shiftRepository.UpdateShiftDetails(shiftDetails);
            }
            else
            {
                List<ShiftDetail> shiftDetails = new List<ShiftDetail>();
                List<ShiftDetailRegion> shiftDetailRegions = new List<ShiftDetailRegion>();
                foreach (int id in ids)
                {
                    ShiftDetail shiftDetail = _shiftRepository.GetShiftDetails(id);
                    shiftDetail.IsDeleted = new BitArray(1, true);
                    shiftDetail.ModifiedBy = aspNetUserId;
                    shiftDetail.ModifiedDate = DateTime.Now;
                    shiftDetails.Add(shiftDetail);
                    ShiftDetailRegion shiftDetailRegion = _shiftRepository.GetShiftDetailRegion(id);
                    shiftDetailRegion.IsDeleted = new BitArray(1, true);
                    shiftDetailRegions.Add(shiftDetailRegion);
                };
                if (await _shiftRepository.UpdateShiftDetails(shiftDetails))
                {
                    return await _shiftRepository.UpdateShiftDetailRegions(shiftDetailRegions);
                }
            }
            return false;
        }

        public SchedulingTableMonthWise MonthWiseScheduling(int regionId, string dateString)
        {
            DateTime date = DateTime.Parse(dateString);
            int startDate = (int)date.DayOfWeek;
            Dictionary<int, List<ShiftDetailsMonthWise>> monthWiseScheduling = new Dictionary<int, List<ShiftDetailsMonthWise>>();
            int totalDays = DateTime.DaysInMonth(date.Year, date.Month);
            _shiftRepository.GetShiftDetailByRegionIdAndDAte(regionId, startDate: date, endDate: date.AddMonths(1).AddDays(-1))
                .ForEach(shiftDetail =>
                {
                    int currentDay = shiftDetail.ShiftDate.Day;
                    if (!monthWiseScheduling.ContainsKey(currentDay))
                    {
                        monthWiseScheduling.Add(currentDay, new List<ShiftDetailsMonthWise>());
                    }
                    monthWiseScheduling[currentDay].Add(new ShiftDetailsMonthWise()
                    {
                        ProviderName = shiftDetail.Shift.Physician.FirstName + " " + shiftDetail.Shift.Physician.LastName,
                        ShiftDetailsId = shiftDetail.ShiftDetailId,
                        StartTime = shiftDetail.StartTime,
                        EndTime = shiftDetail.EndTime,
                        Status = shiftDetail.Status == 0 ? "bg-pink" : "bg-green",
                    });
                });
            return new SchedulingTableMonthWise()
            {
                MonthWise = monthWiseScheduling,
                StartDate = startDate,
                TotalDays = totalDays,
            };
        }

        public List<SchedulingTable> GetSchedulingTableDate(int regionId, int type, string date)
        {
            return type switch
            {
                1 => DayWiseScheduling(DateTime.Parse(date), regionId),
                2 => WeekWiseScheduling(DateTime.Parse(date), regionId),
            };
        }

        private List<SchedulingTable> DayWiseScheduling(DateTime date, int regionId)
        {
            List<SchedulingTable> schedulingTables = new List<SchedulingTable>();
            string path = "/Files//Providers/Photo/";
            _shiftRepository.GetPhysicianWithShiftDetailByRegionIdAndDAte(regionId, date, date)
                .ForEach(physician =>
                {
                    SchedulingTable schedulingTable = new SchedulingTable()
                    {
                        PhysicianId = physician.PhysicianId,
                        Photo = $"{path}{physician.AspNetUserId}/{physician.Photo}",
                        FirstName = physician.FirstName,
                        LastName = physician.LastName,
                        DayWise = new List<ShiftDetailsDayWise>(),
                    };
                    schedulingTables.Add(schedulingTable);
                    foreach (Shift shift in physician.Shifts)
                    {
                        foreach (ShiftDetail shiftDetail in shift.ShiftDetails)
                        {
                            int totalHalfHour = (int)(shiftDetail.EndTime - shiftDetail.StartTime).TotalMinutes / 30;
                            schedulingTable.DayWise.AddRange(
                                Enumerable.Range(shiftDetail.StartTime.Hour, totalHalfHour % 2 == 0 ? totalHalfHour / 2 : (totalHalfHour / 2) + 1)
                                .Select(time => new ShiftDetailsDayWise()
                                {
                                    Status = shiftDetail.Status == 0 ? "#fac4f9" : "#a5cea3",
                                    Time = time,
                                    ShiftDetailsId = shiftDetail.ShiftDetailId,
                                }).ToList()
                            );
                            if (totalHalfHour % 2 == 0)
                            {
                                if (shiftDetail.StartTime.Minute == 30)
                                {
                                    schedulingTable.DayWise.First(shiftDetailsDayWise => shiftDetailsDayWise.Time == shiftDetail.StartTime.Hour)
                                                                                                                                .SecoundHalf = true;
                                    schedulingTable.DayWise.Add(new ShiftDetailsDayWise()
                                    {
                                        Status = shiftDetail.Status == 0 ? "#fac4f9" : "#a5cea3",
                                        Time = shiftDetail.EndTime.Hour,
                                        ShiftDetailsId = shiftDetail.ShiftDetailId,
                                        FirstHalf = true,
                                    });
                                }
                            }
                            else
                            {
                                if (shiftDetail.StartTime.Minute == 30)
                                {
                                    schedulingTable.DayWise.First(shiftDetailsDayWise => shiftDetailsDayWise.Time == shiftDetail.StartTime.Hour)
                                                                                                                              .SecoundHalf = true;
                                }
                                else
                                {
                                    schedulingTable.DayWise.First(shiftDetailsDayWise => shiftDetailsDayWise.Time == shiftDetail.EndTime.Hour)
                                                                                                                                 .FirstHalf = true;
                                }
                            }
                        }
                    }
                });
            return schedulingTables;
        }

        private List<SchedulingTable> WeekWiseScheduling(DateTime date, int regionId)
        {
            List<SchedulingTable> schedulingTables = new List<SchedulingTable>();
            string path = "/Files//Providers/Photo/";
            _shiftRepository.GetPhysicianWithShiftDetailByRegionIdAndDAte(regionId, date, date.AddDays(6))
                .ForEach(physician =>
                {
                    SchedulingTable schedulingTable = new SchedulingTable()
                    {
                        PhysicianId = physician.PhysicianId,
                        Photo = $"{path}{physician.AspNetUserId}/{physician.Photo}",
                        FirstName = physician.FirstName,
                        LastName = physician.LastName,
                        WeekWise = new Dictionary<int, Double>(),
                    };
                    schedulingTables.Add(schedulingTable);
                    foreach (Shift shift in physician.Shifts)
                    {
                        foreach (ShiftDetail shiftDetail in shift.ShiftDetails)
                        {
                            double shiftHours = (shiftDetail.EndTime - shiftDetail.StartTime).TotalHours;
                            if (schedulingTable.WeekWise.ContainsKey((int)shiftDetail.ShiftDate.DayOfWeek))
                            {
                                schedulingTable.WeekWise[(int)shiftDetail.ShiftDate.DayOfWeek] += shiftHours;
                            }
                            else
                            {
                                schedulingTable.WeekWise.Add((int)shiftDetail.ShiftDate.DayOfWeek, shiftHours);
                            }
                        }
                    }
                });
            return schedulingTables;
        }


        private String GenrateHash(String password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(password.Trim()));
                return BitConverter.ToString(hashPassword).Replace("-", "").ToLower();
            }
        }

        private void FilePickUp(String folderName, int aspNetUserId, IFormFile? file)
        {
            String path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/Files/Providers/{folderName}/{aspNetUserId}");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            };
            FileInfo fileInfo = new FileInfo(file!.FileName);
            string fileName = fileInfo.Name;
            string fileNameWithPath = Path.Combine(path, fileName);
            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }

        private string GetFile(String folderName, int aspNetUserId)
        {
            String path = Path.Combine("/Files//Providers/" + folderName + "/" + aspNetUserId.ToString());
            String _path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/Files/Providers/{folderName}/{aspNetUserId}");
            FileInfo[] Files = new DirectoryInfo(_path).GetFiles().OrderBy(p => p.LastWriteTime).ToArray();
            return Path.Combine(path, Files[^1].Name);
        }

    }
}
