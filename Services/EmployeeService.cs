using InterviewTest.Model;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InterviewTest.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly string _connectionString;

        public EmployeeService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            var employees = new List<Employee>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var queryCmd = connection.CreateCommand();
                queryCmd.CommandText = @"SELECT Name, Value FROM Employees ORDER BY Name ASC";

                using (var reader = await queryCmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employees.Add(new Employee
                        {
                            Name = reader.GetString(0),
                            Value = reader.GetInt32(1)
                        });
                    }
                }
            }

            return employees;
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO Employees (Name, Value) VALUES (@name, @value)";
                cmd.Parameters.AddWithValue("@name", employee.Name);
                cmd.Parameters.AddWithValue("@value", employee.Value);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateEmployeeAsync(string name, Employee employee)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"UPDATE Employees SET Value = @value, Name = @newName WHERE Name = @oldName";
                cmd.Parameters.AddWithValue("@newName", employee.Name);
                cmd.Parameters.AddWithValue("@oldName", name);
                cmd.Parameters.AddWithValue("@value", employee.Value);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteEmployeeAsync(string name)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"DELETE FROM Employees WHERE Name = @name";
                cmd.Parameters.AddWithValue("@name", name);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task IncrementValuesAsync(string name)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var queryCmd = connection.CreateCommand();
                queryCmd.CommandText = @"
                UPDATE Employees
                SET Value = CASE 
                    WHEN Name LIKE 'E%' THEN Value + 1
                    WHEN Name LIKE 'G%' THEN Value + 10
                    ELSE Value + 100
                END
                WHERE Name = @name";

                queryCmd.Parameters.AddWithValue("@name", name);
                await queryCmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<(int TotalSum, string Message)> GetSumAsync()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var queryCmd = connection.CreateCommand();
                queryCmd.CommandText = @"
                SELECT SUM(Value) AS TotalSum 
                FROM Employees 
                WHERE Name LIKE 'A%' OR Name LIKE 'B%' OR Name LIKE 'C%'";

                var result = await queryCmd.ExecuteScalarAsync();

                int totalSum = result is DBNull ? 0 : Convert.ToInt32(result);

                if (totalSum >= 11171)
                {
                    return (totalSum, "Sum meets the required threshold.");
                }

                return (totalSum, "Condition failed: total sum is below the required threshold.");
            }
        }
    }
}
