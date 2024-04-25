using Microsoft.AspNetCore.Http;
using Repositories.DataModels;
using Repositories.Interfaces;
using Services.Interfaces;
using System.Collections;
using System.Net;
using System.Net.Mail;

namespace Services.Implementation
{
    public class FileService : IFileService
    {
        private readonly IRequestWiseFileRepository _requestWiseFileRepository;

        public FileService(IRequestWiseFileRepository requestWiseFileRepository)
        {
            _requestWiseFileRepository = requestWiseFileRepository;
        }

        public async Task<bool> AddFile(IFormFile file, int requestId, string firstName, string lastName)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/"+requestId.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            };
            FileInfo fileInfo = new FileInfo(file.FileName);
            string fileName = fileInfo.Name;
            string fileNameWithPath = Path.Combine(path, fileName);
            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            RequestWiseFile requestWiseFile = new()
            {
                RequestId = requestId,
                FileName = fileName,
                CreatedDate = DateTime.Now,
                Uploder = $"{firstName} {lastName}",
                IsDeleted = new BitArray(1, false),
            };
            return await _requestWiseFileRepository.AddFile(requestWiseFile);
        }

        public void SendNewAccountMail(string email, string password)
        {
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("tatva.dotnet.avinashpatel@outlook.com"),
                Subject = "Create Account",
                IsBodyHtml = true,
                Body = $"Successfully, Your Account is Created Email : {email} And Password : {password}",
            };
            //mailMessage.To.Add(email);
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
            }
            catch (Exception ex) { }
        }

    }
}
