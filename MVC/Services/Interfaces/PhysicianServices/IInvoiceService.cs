using Services.ViewModels.Physician;

namespace Services.Interfaces.PhysicianServices
{
    public interface IInvoiceService
    {
        InvoicePage GetInvoice(int aspNetUserId);
    }
}
