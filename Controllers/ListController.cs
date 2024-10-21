using InterviewTest.Model;
using InterviewTest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System;
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

        /// <summary>
        /// Retrieves the list of all employees.
        /// </summary>
        /// <returns>A list of employees in the database.</returns>
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var employees = await _employeeService.GetEmployeesAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a new employee to the database.
        /// </summary>
        /// <param name="employee">The employee object to add.</param>
        /// <returns>Status 200 OK if the employee is added successfully.</returns>
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            if (employee == null)
            {
                return BadRequest("Employee data is null");
            }

            try
            {
                await _employeeService.AddEmployeeAsync(employee);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously updates an existing employee's information.
        /// </summary>
        /// <param name="name">The name of the employee to update.</param>
        /// <param name="employee">The updated <see cref="Employee"/> object containing the new information.</param>
        /// <returns>
        /// A <see cref="IActionResult"/> that represents the result of the update operation:
        /// <list type="bullet">
        /// <item><c>200 OK</c> if the update is successful.</item>
        /// <item><c>404 Not Found</c> if the employee does not exist.</item>
        /// <item><c>400 Bad Request</c> if the input data is invalid.</item>
        /// </list>
        /// </returns>
        [HttpPut("{name}")]
        public async Task<IActionResult> UpdateEmployee(string name, [FromBody] Employee employee)
        {
            if (string.IsNullOrEmpty(name) || employee == null)
            {
                return BadRequest("Invalid input data");
            }

            var updated = await _employeeService.UpdateEmployeeAsync(name, employee);
            if (!updated)
            {
                return NotFound($"Employee with name '{name}' not found");
            }

            return Ok(); // Successfully updated
        }




        /// <summary>
        /// Deletes an employee from the database.
        /// </summary>
        /// <param name="name">The name of the employee to delete.</param>
        /// <returns>Status 200 OK if the deletion is successful, or 404 Not Found if the employee doesn't exist.</returns>
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteEmployee(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Invalid employee name");
            }

            try
            {
                await _employeeService.DeleteEmployeeAsync(name);
                return Ok(); // Assume deletion is successful if no exception is thrown
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Employee with name '{name}' not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Increments the value of employees based on the first letter of their name.
        /// 'E' increments by 1, 'G' by 10, and all others by 100.
        /// </summary>
        /// <param name="name">The name of the employee to increment.</param>
        /// <returns>Status 200 OK if the increment is successful, or 404 Not Found if the employee doesn't exist.</returns>
        [HttpPut("increment/{name}")]
        public async Task<IActionResult> IncrementValues(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Invalid employee name");
            }

            try
            {
                // Call the increment method; no need to assign to a variable
                await _employeeService.IncrementValuesAsync(name);

                // You may want to check if the employee exists before incrementing
                // Depending on your implementation of IncrementValuesAsync,
                // you might want to handle the case where the employee doesn't exist
                return Ok(); // Successfully incremented
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Employee with name '{name}' not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the sum of values where employee names start with 'A', 'B', or 'C', 
        /// and only returns the result if the total sum is greater than or equal to 11171.
        /// </summary>
        /// <returns>The sum of values and a message indicating the result.</returns>
        [HttpGet("sum")]
        public async Task<IActionResult> GetSum()
        {
            try
            {
                var (totalSum, message) = await _employeeService.GetSumAsync();
                return Ok(new { totalSum, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

