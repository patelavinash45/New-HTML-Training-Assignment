using Repositories.DataModels;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.ViewModels;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace Services.Implementation
{
    public class ViewDocumentsServices : IViewDocumentsServices
    {
        private readonly IRequestWiseFileRepository _requestWiseFileRepository;
        private readonly IRequestClientRepository _requestClientRepository;
        private readonly IFileService _fileService;

        public ViewDocumentsServices(IRequestWiseFileRepository requestWiseFileRepository, IRequestClientRepository requestClientRepository,
                                      IFileService fileService)
        {
            _requestWiseFileRepository = requestWiseFileRepository;
            _requestClientRepository = requestClientRepository;
            _fileService = fileService;
        }

        public ViewDocument GetDocumentList(int requestId, int aspNetUserId)
        {
            RequestClient requestClient = _requestClientRepository.GetRequestClientAndRequestByRequestId(requestId);
            return new ViewDocument()
            {
                ConformationNumber = requestClient.Request.ConfirmationNumber,
                FirstName = requestClient.FirstName,
                LastName = requestClient.LastName,
                FileList = _requestWiseFileRepository.GetFilesByRequestId(requestId)
                            .Select(requestWiseFile =>
                            new FileModel()
                            {
                                RequestId = requestId,
                                RequestWiseFileId = requestWiseFile.RequestWiseFileId,
                                FileName = requestWiseFile.FileName,
                                Uploder = requestWiseFile.Uploder,
                                CreatedDate = requestWiseFile.CreatedDate,
                            }).ToList(),
            };
        }

        public async Task<bool> UploadFile(ViewDocument model, String firstName, String lastName, int requestId)
        {
            return await _fileService.AddFile(requestId: requestId, firstName: firstName, lastName: lastName,file: model.File);
        }

        public async Task<bool> DeleteFile(int requestWiseFileId)
        {
            RequestWiseFile requestWiseFile = _requestWiseFileRepository.GetFilesByRequestWiseFileId(requestWiseFileId);
            requestWiseFile.IsDeleted = new BitArray(1, true);
            return await _requestWiseFileRepository.UpdateRequestWiseFiles(new List<RequestWiseFile> { requestWiseFile});
        }

        public async Task<bool> DeleteAllFile(String requestWiseFileIds, int requestId)
        {
            List<int> ids = JsonSerializer.Deserialize<List<String>>(requestWiseFileIds).Select(id => int.Parse(id)).ToList();
            List<RequestWiseFile> requestWiseFiles = _requestWiseFileRepository.GetFilesByRequestId(requestId)
                .Select(requestWiseFile =>
                {
                    requestWiseFile.IsDeleted = new BitArray(1, true);
                    return requestWiseFile;
                }).ToList();
            return await _requestWiseFileRepository.UpdateRequestWiseFiles(requestWiseFiles);
        }

        public bool SendFileMail(String requestWiseFileIds, int requestId)
        {
            List<int> ids = JsonSerializer.Deserialize<List<String>>(requestWiseFileIds).Select(id => int.Parse(id)).ToList();
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("tatva.dotnet.avinashpatel@outlook.com"),
                Subject = "Document List",
                IsBodyHtml = true,
                Body = $"All The Documents For RequestId : {requestId.ToString()}",
            };
            _requestWiseFileRepository.GetRequestWiseFilesByIds(ids)
                .ForEach(requestWiseFile =>
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/Files/{requestWiseFile.RequestId.ToString()}");
                    path = Path.Combine(path, requestWiseFile.FileName);
                    mailMessage.Attachments.Add(new Attachment(path));
                });
            //RequestClient requestClient = _requestClientRepository.GetRequestClientByRequestId(requestId);
            //mailMessage.To.Add(requestClient.Email);
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
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
