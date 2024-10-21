using InterviewTest.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetEmployeesAsync();
    Task AddEmployeeAsync(Employee employee);
    Task<bool> UpdateEmployeeAsync(string name, Employee employee); // Keep this
    Task DeleteEmployeeAsync(string name);
    Task IncrementValuesAsync(string name);
    Task<(int TotalSum, string Message)> GetSumAsync();
    Task<Employee> FindByNameAsync(string name); // Ensure this is defined
}
