using Repositories.DataModels;
using Repositories.Interfaces;
using Services.Interfaces.AdminServices;
using Services.ViewModels.Admin;
using System.Data;
using System.Reflection;
using System.Text.Json;

namespace Services.Implementation.AdminServices
{
    public class RecordService : IRecordService
    {
        private readonly IRequestClientRepository _requestClientRepository;
        private readonly ILogsRepository _logsService;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;

        public RecordService(IRequestClientRepository requestClientRepository,ILogsRepository logsService, IRoleRepository roleRepository,
                                       IUserRepository userRepository)
        {
            _requestClientRepository = requestClientRepository;
            _logsService = logsService;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        public Records GetRecords(Records model)
        {
            Func<RequestClient, bool> predicat = a =>
            (model.RequestType == null || model.RequestType == a.Request.RequestTypeId)
            && (model.Email == null || a.Email.ToLower().Contains(model.Email.ToLower()))
            && (model.Number == null || a.PhoneNumber.Contains(model.Number))
            && (model.StartDate == null || a.Request.AcceptedDate >= model.StartDate)
            && (model.EndDate == null || a.Request.AcceptedDate <= model.EndDate)
            && (model.PatientName == null || a.FirstName.ToLower().Contains(model.PatientName.ToLower())
                                          || a.LastName.ToLower().Contains(model.PatientName.ToLower())
                                          || $"{a.FirstName} {a.LastName}".ToLower().Contains(model.PatientName.ToLower()))
            && (model.ProviderName == null || ( a.Physician != null && (a.Physician.FirstName.ToLower().Contains(model.ProviderName.ToLower())
                                                                     || a.Physician.LastName.ToLower().Contains(model.ProviderName.ToLower()))))
            && (model.RequestStatus == null || a.Status == model.RequestStatus)
            && (a.Status == 3 || a.Status == 7 || a.Status == 8);
            model.RecordTableDatas = _requestClientRepository.GetRequestClientsBasedOnFilter(predicat)
                .Select(requestClient =>
                {
                    RequestNote requestNote = requestClient.Request.RequestNotes.FirstOrDefault(a => a.RequestId == requestClient.RequestId);
                    return new RecordTableData()
                    {
                        RequestId = requestClient.RequestId,
                        PatientName = $"{requestClient.FirstName} {requestClient.LastName}",
                        PhysicianName = requestClient.Physician != null ? $"{requestClient.Physician.FirstName} {requestClient.Physician.LastName}" : "-",
                        Address = $"{requestClient.Street}, {requestClient.City}, {requestClient.State}, {requestClient.ZipCode}",
                        RequestType = requestClient.Request.RequestTypeId,
                        DateOfService = requestClient.Request.AcceptedDate,
                        Email = requestClient.Email,
                        Phone = requestClient.PhoneNumber,
                        Zip = requestClient.ZipCode,
                        Status = requestClient.Status,
                        PhysicianNotes = requestNote != null ? requestNote.PhysicianNotes : "-",
                        AdminNotes = requestNote != null ? requestNote.AdminNotes : "-",
                    };
                }).ToList();
            return model;
        }

        public EmailSmsLogs GetEmailLog(EmailSmsLogs model)
        {
            model.EmailSmsLogTableDatas = GetEmailLogTabledata(model);
            model.Roles = _roleRepository.GetAllRoles().ToDictionary(role => role.RoleId, role => role.Name);
            return model;
        }

        public List<EmailSmsLogTableData> GetEmailLogTabledata(EmailSmsLogs model)
        {
            Func<EmailLog, bool> predicat = a =>
            (model.Email == null || a.EmailId.ToLower().Contains(model.Email))
            && (model.Name == null || a.Name.Contains(model.Name))
            && (model.Role == null || a.RoleId == model.Role)
            && (model.CreatedDate == null || DateOnly.FromDateTime(a.CreateDate) == model.CreatedDate)
            && (model.SendDate == null || DateOnly.FromDateTime(a.SentDate) == model.SendDate);
            return _logsService.GetAllEmailLogs(predicat)
                .Select(emailLog => new EmailSmsLogTableData()
                {
                    Id = (int)emailLog.EmailLogId,
                    Name = emailLog.Name,
                    Action = emailLog.SubjectName,
                    CreatedDate = emailLog.CreateDate,
                    SendDate = emailLog.SentDate,
                    Send = emailLog.IsEmailSent[0] ? "Yes" : "No",
                    Email = emailLog.EmailId,
                    RoleName = emailLog.Role != null ? emailLog.Role.Name : "-",
                }).ToList();
        }

        public EmailSmsLogs GetSMSLog(EmailSmsLogs model)
        {
            model.EmailSmsLogTableDatas = GetSMSLogTabledata(model);
            model.Roles = _roleRepository.GetAllRoles().ToDictionary(role => role.RoleId, role => role.Name);
            return model;
        }

        public List<EmailSmsLogTableData> GetSMSLogTabledata(EmailSmsLogs model)
        {
            Func<Smslog, bool> predicat = a =>
            (model.Phone == null || a.MobileNumber.Contains(model.Phone))
            && (model.Name == null || a.Name.Contains(model.Name))
            && (model.Role == null || a.RoleId == model.Role)
            && (model.CreatedDate == null || DateOnly.FromDateTime(a.CreateDate) == model.CreatedDate)
            && (model.SendDate == null || DateOnly.FromDateTime(a.SentDate) == model.SendDate);
            return _logsService.GetAllSMSLogs(predicat)
                .Select(smslog => new EmailSmsLogTableData()
                {
                    Id = (int)smslog.SmslogId,
                    Name = smslog.Name,
                    Action = smslog.Smstemplate,
                    CreatedDate = smslog.CreateDate,
                    SendDate = smslog.SentDate,
                    Send = smslog.IsSmssent[0] ? "Yes" : "No",
                    RoleName = smslog.Role.Name,
                    Phone = smslog.MobileNumber,
                }).ToList();
        }

        public DataTable ExportAllRecords()
        {
            List<String> columnsNames = new List<String>();
            DataTable dataTable = new DataTable()
            {
                TableName = "RequestDatas",
            };
            int currentRow = 1, index = 1;
            foreach (PropertyInfo propertyInfo in typeof(RequestClient).GetProperties())
            {
                dataTable.Columns.Add(propertyInfo.Name);
                columnsNames.Add(propertyInfo.Name);
                index++;
            }
            Func<RequestClient, bool> predicat = a => (a.Status == 3 || a.Status == 7 || a.Status == 8);
            DataRow row;
            foreach (RequestClient requestClient in _requestClientRepository.GetRequestClientsBasedOnFilter(predicat))
            {
                row = dataTable.NewRow();
                for (int i = 0; i < columnsNames.Count; i++)
                {
                    var value = typeof(RequestClient).GetProperty(columnsNames[i]).GetValue(requestClient);
                    if (value != null)
                    {
                        row[columnsNames[i]] = value.ToString();
                    }
                }
                dataTable.Rows.Add(row);
                currentRow++;
            }
            return dataTable;
        }

        public PatientHistory GetPatientHistory(PatientHistory model, int pageNo)
        {
            int skip = (pageNo - 1) * 5;
            Func<User, bool> predicat = a =>
            (model.Email == null || a.Email.Contains(model.Email))
            && (model.Phone == null || a.Mobile.Contains(model.Phone))
            && (model.FirstName == null || a.FirstName.ToLower().Contains(model.FirstName.ToLower()))
            && (model.LastName == null || a.LastName.ToLower().Contains(model.LastName.ToLower()));
            int totalPatient = _userRepository.CountUsers(predicat);
            List<PatientHistoryTableData> patientHistoryTableDatas = _userRepository.GetAllUser(predicat,skip)
                    .Select(user => new PatientHistoryTableData()
                    {
                        UserId = user.UserId, 
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Phone = user.Mobile,
                        Address = $"{user.Street}, {user.City}, {user.State}, {user.ZipCode}"
                    }).ToList();
            int totalPages = totalPatient % 5 != 0 ? (totalPatient / 5) + 1 : totalPatient / 5;
            model.PatientHistoryTable = new PatientHistoryTable()
            {
                IsFirstPage = pageNo != 1,
                IsLastPage = pageNo != totalPages,
                IsNextPage = pageNo < totalPages,
                IsPreviousPage = pageNo > 1,
                TotalRequests = totalPatient,
                PageNo = pageNo,
                StartRange = skip + 1,
                EndRange = skip + 5 < totalPatient ? skip + 5 : totalPatient,
                PatientHistoryTableDatas = patientHistoryTableDatas,
            };
            return model;
        }

        public PatientHistoryTable GetPatientHistoryTable(string data, int pageNo)
        {
            PatientHistory model = JsonSerializer.Deserialize<PatientHistory>(data);
            return GetPatientHistory(model, pageNo).PatientHistoryTable;
        }

        public PatientRecord GetPatientRecord(int userId, int pageNo)
        {
            int skip = (pageNo - 1) * 5;
            int totalPatient = _requestClientRepository.CountRequestClientsByUserId(userId);
            int totalPages = totalPatient % 5 != 0 ? (totalPatient / 5) + 1 : totalPatient / 5;
            return new PatientRecord()
            {
                IsFirstPage = pageNo != 1,
                IsLastPage = pageNo != totalPages,
                IsNextPage = pageNo < totalPages,
                IsPreviousPage = pageNo > 1,
                TotalRequests = totalPatient,
                PageNo = pageNo,
                StartRange = skip + 1,
                EndRange = skip + 5 < totalPatient ? skip + 5 : totalPatient,
                PatientRecordTableDatas = _requestClientRepository.GetAllRequestClientsByUserId(userId, skip)
                                          .Select(requestClient => new PatientRecordTableData()
                                          {
                                              RequestId = requestClient.RequestId,
                                              Name = $"{requestClient.FirstName} {requestClient.LastName}",
                                              CreatedDate = requestClient.Request.CreatedDate,
                                              Conformation = requestClient.Request.ConfirmationNumber,
                                              ProviderName = requestClient.Physician != null ? $"{requestClient.Physician.FirstName} {requestClient.Physician.LastName}" : "-",
                                              Ststus = requestClient.Status,
                                              Isfinalize = requestClient.Request.Encounters.FirstOrDefault(a => a.RequestId == requestClient.RequestId) != null ?
                                                            (bool)requestClient.Request.Encounters.FirstOrDefault(a => a.RequestId == requestClient.RequestId).IsFinalize : false,
                                              CountDocument = requestClient.Request.RequestWiseFiles != null ? requestClient.Request.RequestWiseFiles.Count : 0,
                                          }).ToList(),
            };
        }

        public BlockHistory GetBlockHistory(BlockHistory model, int pageNo)
        {
            int skip = (pageNo - 1) * 5;
            Func<BlockRequest, bool> predicat = a =>
            (model.Phone == null || a.PhoneNumber.Contains(model.Phone))
            && (model.Email == null || a.Email.Contains(model.Email))
            && (model.Name == null || a.Request.RequestClients.FirstOrDefault(x => x.RequestId == a.RequestId).FirstName.ToLower().Contains(model.Name.ToLower())
                                   || a.Request.RequestClients.FirstOrDefault(x => x.RequestId == a.RequestId).LastName.ToLower().Contains(model.Name.ToLower()))
            && (model.Date == null || DateOnly.FromDateTime(a.CreatedDate) == model.Date);
            int totalPatient = _requestClientRepository.CountRequestClientsAndBlockRequestBasedOnFilter(predicat);
            int totalPages = totalPatient % 5 != 0 ? (totalPatient / 5) + 1 : totalPatient / 5;
            model.BlockHistoryTable = new BlockHistoryTable()
            {
                IsFirstPage = pageNo != 1,
                IsLastPage = pageNo != totalPages,
                IsNextPage = pageNo < totalPages,
                IsPreviousPage = pageNo > 1,
                TotalRequests = totalPatient,
                PageNo = pageNo,
                StartRange = skip + 1,
                EndRange = skip + 5 < totalPatient ? skip + 5 : totalPatient,
                BlockHistoryTableDatas = _requestClientRepository.GetRequestClientsAndBlockRequestBasedOnFilter(predicat)
                                          .Select(blockRequest => new BlockHistoryTableData()
                                          {
                                                  Name = $"{blockRequest.Request.RequestClients.FirstOrDefault(a => a.RequestId == blockRequest.RequestId).FirstName} " +
                                                                                         $"{blockRequest.Request.RequestClients.FirstOrDefault(a => a.RequestId == blockRequest.RequestId).LastName}",
                                                  Email = blockRequest.Email,
                                                  Phone = blockRequest.PhoneNumber,
                                                  CratedDate = blockRequest.CreatedDate,
                                                  Notes = blockRequest.Reason != null ? blockRequest.Reason : "-",
                                                  RequestId = (int)blockRequest.RequestId,
                                                  IsActive = blockRequest.IsActive[0],
                                          }).ToList(),
            };
            return model;
        }

        public BlockHistoryTable GetBlockHistoryTable(string data, int pageNo, string date)
        {
            BlockHistory model = JsonSerializer.Deserialize<BlockHistory>(data);
            if(date != null) { 
                model.Date = DateOnly.Parse(date);
            }
            return GetBlockHistory(model, pageNo).BlockHistoryTable;
        }

        public async Task<bool> UnblockRequest(int requestId)
        {
            if(await _requestClientRepository.DeleteBlockRequest(requestId))
            {
                RequestClient requestClient = _requestClientRepository.GetRequestClientByRequestId(requestId);
                requestClient.Status = 1;
                requestClient.PhysicianId = null;
                return await _requestClientRepository.UpdateRequestClient(requestClient);
            }
            return false;
        }
    }
}
