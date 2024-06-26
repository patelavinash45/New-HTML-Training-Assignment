﻿using Services.ViewModels.Admin;
using Services.ViewModels.Physician;

namespace Services.Interfaces.AdminServices
{
    public interface IProvidersService
    {
        List<ProviderLocation> GetProviderLocation();

        Provider GetProviders(int regionId);

        Task<bool> EditProviderNotification(int providerId, bool isNotification);

        Task<bool> ContactProvider(ContactProvider model);

        CreateProvider GetCreateProvider();

        EditProvider GetEditProvider(int physicianId);

        Task<String> CreateProvider(CreateProvider model);

        ProviderScheduling GetProviderSchedulingData();

        List<SchedulingTable> GetSchedulingTableDate(int regionId, int type, string date);

        SchedulingTableMonthWise MonthWiseScheduling(int regionId, String dateString);

        Task<bool> CreateShift(CreateShift model, int aspNetUserId, bool isAdmin);

        RequestedShift GetRequestedShift();

        RequestShiftModel GetRequestShiftTableDate(int regionId, bool isMonth, int pageNo);

        Task<bool> ChangeShiftDetails(string dataList, bool isApprove, int aspNetUserId);

        ViewShift GetShiftDetails(int shiftDetailsId);

        Task<bool> EditShiftDetails(string data, int aspNetUserId);

        ProviderOnCall GetProviderOnCall(int regionId);

        ProviderList GetProviderList(int regionId);

        Task<bool> SaveSign(string sign, int physicianId);

        Task<bool> EditPhysicianAccountInformation(EditProvider model, int physicianId, int aspNetUserId);

        Task<bool> EditPhysicianPhysicianInformation(EditProvider model, int physicianId, int aspNetUserId);

        Task<bool> EditPhysicianMailAndBillingInformation(EditProvider model, int physicianId, int aspNetUserId);

        Task<bool> EditPhysicianProviderProfile(EditProvider model, int physicianId, int aspNetUserId);

        Task<bool> EditPhysicianOnbordingInformation(EditProvider model, int physicianId, int aspNetUserId);

        InvoicePage GetInvoiceDetails(int physicianId, string date);

        Task<bool> ApproveInvoice(int invoiceId, double totalAmount, double bounsAmount, string notes);

        CreateInvoice GetWeeklyTimeSheet(int physicianId, string date);

        Receipts GetReceipts(int physicianId, string date);

        PayRate GetPayRate(int physicianId);

        Task<bool> EditPayRate(PayRate model, int physicianId, int aspNetUserId);
    }
}
