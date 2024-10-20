using InterviewTest.Model;
using InterviewTest.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InterviewTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public ListController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // Get all employees
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _employeeService.GetEmployeesAsync();
            return Ok(employees);
        }

        // Add a new employee
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            await _employeeService.AddEmployeeAsync(employee);
            return Ok();
        }

        // Update an existing employee
        [HttpPut("{name}")]
        public async Task<IActionResult> UpdateEmployee(string name, [FromBody] Employee employee)
        {
            await _employeeService.UpdateEmployeeAsync(name, employee);
            return Ok();
        }

        // Remove an employee
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteEmployee(string name)
        {
            await _employeeService.DeleteEmployeeAsync(name);
            return Ok();
        }

        // Increment values based on the first letter of the Name
        [HttpPut("increment/{name}")]
        public async Task<IActionResult> IncrementValues(string name)
        {
            await _employeeService.IncrementValuesAsync(name);
            return Ok();
        }

        // Sum Values where Names start with A, B, or C
        [HttpGet("sum")]
        public async Task<IActionResult> GetSum()
        {
            var (totalSum, message) = await _employeeService.GetSumAsync();
            return Ok(new { totalSum, message });
        }
    }
}
