using InterviewTest.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InterviewTest.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task AddEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(string name, Employee employee);
        Task DeleteEmployeeAsync(string name);
        Task IncrementValuesAsync(string name);
        Task<(int TotalSum, string Message)> GetSumAsync();
    }
}
