using Repositories.DataModels;
using Repositories.Interface;
using Services.Interface;
using Services.ViewModels;

namespace Services.Implementation
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository) 
        { 
            _studentRepository = studentRepository;
        }

        public Dashboard getStudentsData(int pageNo, int CurrentPageSize, string searchElement)
        {
            int skip = (pageNo - 1) * CurrentPageSize;
            int totalStudents = _studentRepository.countStudents(searchElement);
            int totalPages = (totalStudents%CurrentPageSize) == 0 ? totalStudents / CurrentPageSize : (totalStudents/CurrentPageSize) +1;
            StudentData studentData = new StudentData()
            {
                CourseList = _studentRepository.getAllCourses().Select(course => course.Name).ToList(),
            };
            return new Dashboard()
            {
                PageNo = pageNo,
                CurrentPageSize = CurrentPageSize,
                TotalStudents = totalStudents,
                IsNext = pageNo < totalPages,
                IsPrevious = pageNo > 1,
                StudentData = studentData,
                StudentsRecords = getDashboardTableData(skip, CurrentPageSize, searchElement),
            };
        }

        public List<DashboardTableData> getDashboardTableData(int skip, int CurrentPageSize, string searchElement)
        {
            return _studentRepository.getAllStudents(skip, CurrentPageSize, searchElement)
                    .Select(student => new DashboardTableData()
                    {
                        Id = student.Id,
                        Firstname =student.Firstname, 
                        Lastname =student.Lastname,
                        Email = student.Email,
                        Age = student.Age,
                        Gender = student.Gender,
                        Course = student.Course,
                        Grade = student.Grade,
                    }).ToList();
        }

        public async Task<bool> deleteStudent(int id)
        {
            Student student = _studentRepository.GetStudent(id);
            student.IsDelete = true;
            return await _studentRepository.updateStudent(student);
        }

        public StudentData getStudentData(int id)
        {
            Student student = _studentRepository.GetStudent(id);
            return new StudentData()
            {
                Firstname = student.Firstname,
                Lastname = student.Lastname,
                Email = student.Email,
                Course = student.Course,
                Gender = student.Gender,
                Grade = student.Grade,
                Date = ((DateTime)student.BirthDate).ToString("yyyy-MM-dd"),
            };
        }

        public async Task<bool> addStudent(StudentData model)
        {
            Course course = _studentRepository.checkCourse(model.Course);
            if(course == null)
            {
                course = new Course()
                {
                    Name = model.Course,
                };
                if(await _studentRepository.addCourse(course))
                {
                    Student student = new Student()
                    {
                        Courseid = course.Id,
                        Firstname = model.Firstname,
                        Lastname = model.Lastname,
                        Email = model.Email,
                        Age = CalculateAge(model.DateofBirth),
                        Gender = model.Gender,
                        Course = model.Course,
                        Grade = model.Grade,
                        IsDelete = false,
                        BirthDate = model.DateofBirth,
                    };
                    return await _studentRepository.addStudent(student);
                }
            }
            else
            {
                Student student = new Student()
                {
                    Courseid = course.Id,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    Email = model.Email,
                    Age = CalculateAge(model.DateofBirth),
                    Gender = model.Gender,
                    Course = model.Course,
                    Grade = model.Grade,
                    IsDelete = false,
                    BirthDate = model.DateofBirth,
                };
                return await _studentRepository.addStudent(student);
            }
            return false;
        }

        public async Task<bool> updateStudent(StudentData model, int id)
        {
            Course course = _studentRepository.checkCourse(model.Course);
            if (course == null)
            {
                course = new Course()
                {
                    Name = model.Course,
                };
                if (await _studentRepository.addCourse(course))
                {
                    Student student = _studentRepository.GetStudent(id);
                    student.Courseid = course.Id;
                    student.Firstname = model.Firstname;
                    student.Lastname = model.Lastname;
                    student.Email = model.Email;
                    student.Age = CalculateAge(model.DateofBirth);
                    student.Gender = model.Gender;
                    student.Course = model.Course;
                    student.Grade = model.Grade;
                    student.BirthDate = model.DateofBirth;
                    return await _studentRepository.updateStudent(student);
                }
            }
            else
            {
                Student student = _studentRepository.GetStudent(id);
                student.Courseid = course.Id;
                student.Firstname = model.Firstname;
                student.Lastname = model.Lastname;
                student.Email = model.Email;
                student.Age = CalculateAge(model.DateofBirth);
                student.Gender = model.Gender;
                student.Course = model.Course;
                student.Grade = model.Grade;
                return await _studentRepository.updateStudent(student);
            }
            return false;
        }

        private short CalculateAge(DateTime? dateOfBirth)
        {
            int age = 0;
            age = DateTime.Now.Year - dateOfBirth.Value.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.Value.DayOfYear)
            {
                age = age - 1;
            }
            return (short)age;
        }
    }
}
