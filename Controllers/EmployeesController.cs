using InterviewTest.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace InterviewTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {
        // Use SqliteConnectionStringBuilder to create the connection string
        private readonly SqliteConnectionStringBuilder _connectionStringBuilder;

        public EmployeesController()
        {
            // Set up the connection string with a relative path
            _connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = "./SqliteDB.db" // Relative path to the database
            };
        }

        // Get all employees
        [HttpGet]
        public IActionResult GetEmployees()
        {
            var employees = new List<Employee>();

            using (var connection = new SqliteConnection(_connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                var queryCmd = connection.CreateCommand();
                queryCmd.CommandText = @"SELECT Name, Value FROM Employees ORDER BY Name ASC";

                using (var reader = queryCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Name = reader.GetString(0),
                            Value = reader.GetInt32(1)
                        });
                    }
                }
            }

            return Ok(employees);
        }
    }
}