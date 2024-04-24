using Repositories.DataModels;

namespace Repositories.Interface
{
    public interface IStudentRepository
    {
        List<Course> getAllCourses();

        List<Student> getAllStudents(int skip, int records, string searchElement);

        int countStudents(string searchElement);

        Course checkCourse(string course);

        Student GetStudent(int id);

        Task<bool> addCourse(Course course);

        Task<bool> addStudent(Student student);

        Task<bool> updateStudent(Student student);
    }
}
