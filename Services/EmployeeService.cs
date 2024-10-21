using InterviewTest.Model;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace InterviewTest.Services
{
    /// <summary>
    /// Provides methods to manage employee data in the database.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeService"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration for accessing the connection string.</param>
        public EmployeeService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Retrieves all employees from the database.
        /// </summary>
        /// <returns>A list of <see cref="Employee"/> objects.</returns>
        /// <exception cref="Exception">Throws an exception if there's an error while fetching employees.</exception>
        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            var employees = new List<Employee>();

            try
            {
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
            }
            catch (DbException dbEx)
            {
                throw new Exception("Error occurred while fetching employees.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }

            return employees;
        }

        /// <summary>
        /// Adds a new employee to the database.
        /// </summary>
        /// <param name="employee">The <see cref="Employee"/> object to add.</param>
        /// <exception cref="Exception">Throws an exception if there's an error while adding the employee.</exception>
        public async Task AddEmployeeAsync(Employee employee)
        {
            try
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
            catch (DbException dbEx)
            {
                throw new Exception("Error occurred while adding employee.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        /// <summary>
        /// Asynchronously updates an existing employee's information in the database.
        /// </summary>
        /// <param name="name">The current name of the employee to update.</param>
        /// <param name="employee">The updated <see cref="Employee"/> object containing the new information.</param>
        /// <returns>
        /// <c>true</c> if the update is successful; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="employee"/> is <c>null</c>.</exception>
        /// <exception cref="Exception">Throws an exception if there is an error while updating the employee.</exception>
        public async Task<bool> UpdateEmployeeAsync(string name, Employee employee)
        {
            // Ensure the employee to be updated is valid
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee), "Employee cannot be null.");
            }

            // Check if the employee exists before trying to update
            var existingEmployee = await FindByNameAsync(name);
            if (existingEmployee == null)
            {
                return false; // Employee not found
            }

            // Update the employee in the database
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Prepare the update command
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"UPDATE Employees SET Name = @newName, Value = @value WHERE Name = @oldName";
                    cmd.Parameters.AddWithValue("@newName", employee.Name);
                    cmd.Parameters.AddWithValue("@value", employee.Value);
                    cmd.Parameters.AddWithValue("@oldName", name);

                    // Execute the command and check how many rows were affected
                    var rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0; // Return true if the update was successful
                }
            }
            catch (DbException dbEx)
            {
                // Handle database exceptions
                throw new Exception("A database error occurred while updating the employee.", dbEx);
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                throw new Exception("An error occurred while updating the employee.", ex);
            }
        }


        /// <summary>
        /// Asynchronously finds an employee by their name.
        /// </summary>
        /// <param name="name">The name of the employee to find.</param>
        /// <returns>
        /// A <see cref="Task{Employee}"/> representing the asynchronous operation, 
        /// with a value of the found <see cref="Employee"/> object if found; otherwise, <c>null</c>.
        /// </returns>
        /// <exception cref="Exception">Throws an exception if an error occurs while retrieving the employee.</exception>
        public async Task<Employee> FindByNameAsync(string name)
        {
            Employee employee = null;

            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"SELECT Name, Value FROM Employees WHERE Name = @name";
                    cmd.Parameters.AddWithValue("@name", name);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            employee = new Employee
                            {
                                Name = reader.GetString(0),
                                Value = reader.GetInt32(1)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (log, rethrow, etc.)
                throw new Exception("An error occurred while retrieving the employee.", ex);
            }

            return employee;
        }






        /// <summary>
        /// Deletes an employee from the database by their name.
        /// </summary>
        /// <param name="name">The name of the employee to delete.</param>
        /// <exception cref="Exception">Throws an exception if there's an error while deleting the employee.</exception>
        public async Task DeleteEmployeeAsync(string name)
        {
            try
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
            catch (DbException dbEx)
            {
                throw new Exception("Error occurred while deleting employee.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        /// <summary>
        /// Increments the value of an employee based on their name's first letter.
        /// </summary>
        /// <param name="name">The name of the employee to increment the value for.</param>
        /// <exception cref="Exception">Throws an exception if there's an error while updating employee values.</exception>
        public async Task IncrementValuesAsync(string name)
        {
            try
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
            catch (DbException dbEx)
            {
                throw new Exception("Error occurred while incrementing employee values.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        /// <summary>
        /// Retrieves the sum of employee values where the name starts with 'A', 'B', or 'C'.
        /// </summary>
        /// <returns>A tuple containing the total sum and a message about the sum.</returns>
        /// <exception cref="Exception">Throws an exception if there's an error while calculating the sum.</exception>
        public async Task<(int TotalSum, string Message)> GetSumAsync()
        {
            try
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
            catch (DbException dbEx)
            {
                throw new Exception("Error occurred while calculating the sum.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }
    }
}
