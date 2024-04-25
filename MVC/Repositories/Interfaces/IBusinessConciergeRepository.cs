using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IBusinessConciergeRepository
    {
        Task<int> AddBusiness(Business business);

        Task<int> AddConcierge(Concierge concierge);

        Task<int> AddRequestConcierge(RequestConcierge requestConcierge);

        Task<int> AddRequestBusiness(RequestBusiness requestBusiness);
    }
}
