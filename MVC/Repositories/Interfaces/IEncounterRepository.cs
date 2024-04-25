using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IEncounterRepository
    {
        Encounter GetEncounter(int requestId);

        Task<bool> AddEncounter(Encounter encounter);

        Task<bool> UpdateEncounter(Encounter encounter);
    }
}
