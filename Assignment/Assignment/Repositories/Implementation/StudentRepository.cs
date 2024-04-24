using Repositories.DataContext;
using Repositories.DataModels;
using Repositories.Interface;

namespace Repositories.Implementation
{
    public class StudentRepository : IStudentRepository
    {
        private readonly StudentDbContext _dbContext;

        public StudentRepository(StudentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Course> getAllCourses()
        {
            return _dbContext.Courses.ToList();
        }

        public List<Student> getAllStudents(int skip, int records, string searchElement)
        {
            Func<Student, bool> predicates =
                a => (searchElement == null || a.Firstname.ToLower().Contains(searchElement.ToLower()) 
                                            || a.Lastname.ToLower().Contains(searchElement.ToLower()) 
                                            || $"{a.Firstname} {a.Lastname}".ToLower().Contains(searchElement.ToLower())
                                            || $"{a.Lastname} {a.Firstname}".ToLower().Contains(searchElement.ToLower()))
                         && a.IsDelete != true;
            return _dbContext.Students.Where(predicates).OrderBy(a => a.Id).Skip(skip).Take(records).ToList();
        }

        public int countStudents(string searchElement)
        {
            Func<Student, bool> predicates =
                a => (searchElement == null || a.Firstname.ToLower().Contains(searchElement.ToLower())
                                            || a.Lastname.ToLower().Contains(searchElement.ToLower())
                                            || $"{a.Firstname} {a.Lastname}".ToLower().Contains(searchElement.ToLower())
                                            || $"{a.Lastname} {a.Firstname}".ToLower().Contains(searchElement.ToLower()))
                         && a.IsDelete != true;
            return _dbContext.Students.Count(predicates);
        }

        public Course checkCourse(string course)
        {
            return _dbContext.Courses.FirstOrDefault(a => a.Name == course);
        }

        public Student GetStudent(int id)
        {
            return _dbContext.Students.FirstOrDefault(a => a.Id == id);
        }

        public async Task<bool> addCourse(Course course)
        {
            _dbContext.Courses.Add(course);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> addStudent(Student student)
        {
            _dbContext.Students.Add(student);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> updateStudent(Student student)
        {
            _dbContext.Students.Update(student);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
