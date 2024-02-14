using Authorization_and_Authentication.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authorization_and_Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _ApplicationDbContext;

        public EmployeeController(ApplicationDbContext ApplicationDbContext)
        {
            _ApplicationDbContext = ApplicationDbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_ApplicationDbContext.Employee.ToList());
        }


        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(_ApplicationDbContext.Employee.FirstOrDefault(c => c.EmpId == id));
        }


        [HttpPost]
        public IActionResult Post([FromBody] Employee employee)
        {
            _ApplicationDbContext.Employee.Add(employee);
            _ApplicationDbContext.SaveChanges();

            return Ok("Employee created");
        }


        [HttpPut]
        public IActionResult Put([FromBody] Employee employee)
        {
            var emp = _ApplicationDbContext.Employee.FirstOrDefault(c => c.EmpId == employee.EmpId);

            if (emp == null)
                return BadRequest();

            emp.FirstName = employee.FirstName;
            emp.LastName = employee.LastName;
            emp.Email = employee.Email;
            emp.PhoneNumber = employee.PhoneNumber;

            _ApplicationDbContext.Employee.Update(emp);
            _ApplicationDbContext.SaveChanges();

            return Ok("Employee updated");
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var emp = _ApplicationDbContext.Employee.FirstOrDefault(c => c.EmpId == id);

            if (emp == null)
                return BadRequest();

            _ApplicationDbContext.Employee.Remove(emp);
            _ApplicationDbContext.SaveChanges();

            return Ok("Employee deleted");
        }


    }
}
