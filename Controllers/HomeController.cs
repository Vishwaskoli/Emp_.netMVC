using ADO.Data;
using ADO.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ADO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private AppDBContext _dbContext;

        public HomeController(ILogger<HomeController> logger, AppDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            List<Employee> employees = new List<Employee>();
            employees = _dbContext.GetEmployees();
            ViewBag.Department = _dbContext.GetDepartments();
            ViewBag.Desgination = _dbContext.GetAllDesignations();

            return View(employees);
        }

        public IActionResult Details(int id)
        {
            Employee e1 = _dbContext.GetEmployee(id);
            ViewBag.Department = _dbContext.GetDepartmentbyId(e1.DeptID);
            ViewBag.Desgination = _dbContext.GetDesignationbyId(e1.DesgID);
            return View(e1);
        }

        public IActionResult Create()
        {
            ViewBag.Departments = _dbContext.GetDepartments();
            return View();
        }


        [HttpPost]
        public IActionResult Create(Employee employee,IFormFile Img)
        {
            if (Img != null && Img.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    Img.CopyTo(memoryStream);
                    employee.ProfileImg = memoryStream.ToArray();
                }
            }
            bool cr = _dbContext.CreateEmployee(employee);
            if (!cr)
                return View(employee);
            else
                return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Employee emp = _dbContext.GetEmployee(id);
            ViewBag.Departments = _dbContext.GetDepartments();
            ViewBag.Designations = _dbContext.GetDesignationsByDept(emp.DeptID);
            return View(emp);
        }

        [HttpPost]
        public IActionResult Edit(Employee employee, IFormFile Img)
        {
            if (Img != null && Img.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    Img.CopyTo(ms);
                    employee.ProfileImg = ms.ToArray();
                }
            }

            bool updated = _dbContext.UpdateEmployee(employee);
            if (!updated)
                return View(employee);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Employee emp = _dbContext.GetEmployee(id);
            return View(emp);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _dbContext.DeleteEmployee(id);
            return RedirectToAction("Index");
        }

        public JsonResult GetDesignations(int deptId)
        {
            var list = _dbContext.GetDesignationsByDept(deptId);
            return Json(list);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
