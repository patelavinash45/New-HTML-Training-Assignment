using Services.ViewModels;

namespace Services.Interface
{
    public interface IStudentService
    {
        Dashboard getStudentsData(int pageNo, int CurrentPageSize, string searchElement);

        Task<bool> addStudent(StudentData model);

        Task<bool> updateStudent(StudentData model, int id);

        StudentData getStudentData(int id);

        Task<bool> deleteStudent(int id);
    }
}
