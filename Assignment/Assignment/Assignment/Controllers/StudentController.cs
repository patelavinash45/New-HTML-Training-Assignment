using Microsoft.AspNetCore.Mvc;
using Repositories.DataModels;
using Services.Interface;
using Services.ViewModels;

namespace Assignment.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        public IActionResult Dashboard()
        {
            HttpContext.Session.SetInt32("id", 0);
            var records = HttpContext.Session.GetInt32("records");
            if(records == null)
            {
                records = 5;
            }
            return View(_studentService.getStudentsData(1,records.Value,null));
        }

        [HttpGet]
        public IActionResult GetTabledata(int pageNo,int records, string searchElement)
        {
            HttpContext.Session.SetInt32("records",records);
            return PartialView("_StudentsRecords", _studentService.getStudentsData(pageNo,records,searchElement));
        }

        public async Task<IActionResult> Student(StudentData student)
        {
            int id = HttpContext.Session.GetInt32("id").Value;
            if(id == 0)
            {
                await _studentService.addStudent(student);
            }
            else
            {
                await _studentService.updateStudent(student, id);
                HttpContext.Session.SetInt32("id", 0);
            }
            return RedirectToAction("Dashboard", "Student");
        }

        public JsonResult GetStudentData(int id)
        {
            HttpContext.Session.SetInt32("id", id);
            return Json(new { data = _studentService.getStudentData(id) });
        }

        public async Task<IActionResult> DeleteStudent(int id)
        {
            await _studentService.deleteStudent(id);
            return RedirectToAction("Dashboard", "Student");
        }
    }
}
