using Services.ViewModels.Admin;

namespace Services.Interfaces.AdminServices
{
    public interface ICloseCaseService
    {
        CloseCase GetDetails(int requestId);

        Task<bool> UpdateDetails(CloseCase model, int requestId);

        Task<bool> RequestAddToCloseCase(int requestId);
    }
}
