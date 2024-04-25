using Repositories.DataModels;

namespace Repositories.Interfaces
{
    public interface IRequestNotesRepository
    {
        RequestNote GetRequestNoteByRequestId(int requestId);

        Task<bool> AddRequestNote(RequestNote requestNote);

        Task<bool> UpdateRequestNote(RequestNote requestNote);
    }
}
