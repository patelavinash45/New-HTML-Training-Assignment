﻿using Microsoft.AspNetCore.Http;
using Repositories.DataModels;
using Repositories.Interfaces;
using Services.Interfaces.AdminServices;
using Services.Interfaces.AuthServices;
using Services.ViewModels.Admin;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace Services.Implementation.AdminServices
{
    public class ViewNotesService : IViewNotesService
    {
        private readonly IRequestNotesRepository _requestNotesRepository;
        private readonly IRequestStatusLogRepository _requestSatatusLogRepository;
        private readonly IRequestClientRepository _requestClientRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IJwtService _jwtService;

        public ViewNotesService(IRequestNotesRepository requestNotesRepository, IRequestStatusLogRepository requestSatatusLogRepository, 
                                      IRequestClientRepository requestClientRepository, IRequestRepository requestRepository, 
                                      IJwtService jwtService)
        {
            _requestNotesRepository = requestNotesRepository;
            _requestSatatusLogRepository = requestSatatusLogRepository;
            _requestRepository = requestRepository;
            _requestClientRepository = requestClientRepository;
            _jwtService = jwtService;
        }

        public ViewNotes GetNotes(int RequestId)
        {
            RequestNote requestNote = _requestNotesRepository.GetRequestNoteByRequestId(RequestId);
            return new ViewNotes()
            {
                RequestId = RequestId,
                AdminNotes = requestNote != null ? requestNote.AdminNotes : null,
                PhysicianNotes = requestNote != null ? requestNote.PhysicianNotes : null,
                TransferNotes = _requestSatatusLogRepository.GetRequestStatusLogByRequestId(RequestId)
                                .Select(requestStatusLog =>new Tuple<string, string>(requestStatusLog.CreatedDate.ToString("MMM dd,yyy"),
                                                                                     requestStatusLog.Notes != null ? requestStatusLog.Notes : "")).ToList(),
            };
        }

        public async Task<bool> AddAdminNotes(String newNotes, int requestId, int aspNetUserId, bool isAdmin)
        {
            RequestNote requestNote = _requestNotesRepository.GetRequestNoteByRequestId(requestId);
            if(requestNote == null)
            {
                return await _requestNotesRepository
                    .AddRequestNote(new RequestNote()
                    {
                        RequestId = requestId,
                        AdminNotes = isAdmin ? newNotes : null,
                        PhysicianNotes = isAdmin ? null : newNotes,
                        CreatedDate = DateTime.Now,
                    });
            }
            requestNote.AdminNotes = isAdmin ? newNotes : null;
            requestNote.PhysicianNotes = isAdmin ? null : newNotes;
            requestNote.ModifiedDate = DateTime.Now;
            requestNote.ModifiedBy = aspNetUserId;
            return await _requestNotesRepository.UpdateRequestNote(requestNote);
        }

        public async Task<bool> CancelRequest(CancelPopUp model)
        {
            RequestClient requestClient = _requestClientRepository.GetRequestClientByRequestId(model.RequestId);
            requestClient.Status = 3;
            if(await _requestClientRepository.UpdateRequestClient(requestClient))
            {
                Request request = _requestRepository.GetRequestByRequestId(model.RequestId);
                request.CaseTagId = model.Reason;
                request.ModifiedDate = DateTime.Now;
                if(await _requestRepository.UpdateRequest(request))
                {
                    return await _requestSatatusLogRepository
                        .AddRequestSatatusLog(new RequestStatusLog()
                        {
                            RequestId = model.RequestId,
                            Status = 3,
                            CreatedDate = DateTime.Now,
                            Notes = model.AdminTransferNotes,
                        });
                }
            }
            return false;
        }

        public async Task<bool> AgreementDeclined(Agreement model)
        {
            RequestClient requestClient = _requestClientRepository.GetRequestClientByRequestId(model.RequestId);
            requestClient.Status = 8;
            if(await _requestClientRepository.UpdateRequestClient(requestClient))
            {
                return await _requestSatatusLogRepository
                    .AddRequestSatatusLog( new RequestStatusLog()
                    {
                        RequestId = model.RequestId,
                        Status = 8,
                        CreatedDate = DateTime.Now,
                        Notes = model.CancelationReson,
                    });
            }
            return false;
        }

        public async Task<bool> AgreementAgree(Agreement model)
        {
            RequestClient requestClient = _requestClientRepository.GetRequestClientByRequestId(model.RequestId);
            requestClient.Status = 5;
            if(await _requestClientRepository.UpdateRequestClient(requestClient))
            {
                Request request = _requestRepository.GetRequestByRequestId(model.RequestId);
                request.AcceptedDate = DateTime.Now;
                request.ModifiedDate = DateTime.Now;
                return await _requestRepository.UpdateRequest(request);
            }
            return false;
        }

        public async Task<bool> AssignRequest(AssignAndTransferPopUp model)
        {
            RequestClient requestClient = _requestClientRepository.GetRequestClientByRequestId(model.RequestId);
            requestClient.Status = 1;
            requestClient.PhysicianId = model.SelectedPhysician;
            if(await _requestClientRepository.UpdateRequestClient(requestClient))
            {
                return await _requestSatatusLogRepository
                    .AddRequestSatatusLog(new RequestStatusLog()
                    {
                        RequestId = model.RequestId,
                        Status = 1,
                        CreatedDate = DateTime.Now,
                        Notes = model.AdminTransferNotes,
                        PhysicianId = model.SelectedPhysician,
                    });
            }
            return false;
        }

        public async Task<bool> BlockRequest(BlockPopUp model)
        {
            RequestClient requestClient = _requestClientRepository.GetRequestClientByRequestId(model.RequestId);
            requestClient.Status = 0;
            if(await _requestClientRepository.UpdateRequestClient(requestClient))
            {
                BlockRequest blockRequest = new()
                {
                    PhoneNumber = requestClient.PhoneNumber,
                    Email = requestClient.Email,
                    Reason = model.AdminTransferNotes,
                    RequestId = model.RequestId,
                    CreatedDate = DateTime.Now,
                    IsActive = new BitArray(1, false),
                };
                if (await _requestSatatusLogRepository.AddBlockRequest(blockRequest))
                {
                    return await _requestSatatusLogRepository
                        .AddRequestSatatusLog(new RequestStatusLog()
                        {
                            RequestId = model.RequestId,
                            Status = 0,
                            CreatedDate = DateTime.Now,
                            Notes = model.AdminTransferNotes,
                        });
                }
            }
            return false;
        }

        public async Task<bool> ClearRequest(int requestId)
        {
            RequestClient requestClient = _requestClientRepository.GetRequestClientByRequestId(requestId);
            requestClient.Status = 12;
            return await _requestClientRepository.UpdateRequestClient(requestClient);
        }

        public bool SendAgreement(Agreement model,HttpContext httpContext)
        {
            var request = httpContext.Request;
            List<Claim> claims = new List<Claim>()
            {
                new Claim("requestId", model.RequestId.ToString()),
            };
            String token = _jwtService.GenrateJwtTokenForSendMail(claims, DateTime.Now.AddDays(2));
            String link = $"{request.Scheme}://{request.Host}/Admin/Agreement?token={token}";
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("tatva.dotnet.avinashpatel@outlook.com"),
                Subject = "Agreement",
                IsBodyHtml = true,
                Body = "To Further Proceed to your Request : " + link,
            };
            //mailMessage.To.Add(model.Email);
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
